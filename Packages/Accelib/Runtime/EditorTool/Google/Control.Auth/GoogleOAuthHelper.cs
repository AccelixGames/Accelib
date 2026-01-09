#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using Accelib.Data;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

namespace Accelib.EditorTool.Google.Control.Auth
{
    [CreateAssetMenu(fileName = "(Google) OAuthHelper", menuName = "Accelib.Google/OAuthHelper", order = 0)]
    public class GoogleOAuthHelper : ScriptableObjectCached<GoogleOAuthHelper>
    {
        private const string AuthUrl = "https://accounts.google.com/o/oauth2/v2/auth";
        private const string TokenUrl = "https://oauth2.googleapis.com/token";
        private const string TokenInfoUrl = "https://www.googleapis.com/oauth2/v3/tokeninfo?access_token=";

        private string RedirectUri => $"http://127.0.0.1:{port}/";
        
        private static string AccessTokenPrefsKey => $"GSHEETS_ACCESS_TOKEN";
        private static string RefreshTokenPrefsKey => $"GSHEETS_REFRESH_TOKEN";
        private static string UpdatedAtPrefsKey => $"GSHEETS_UPDATED_AT";
        private static string TokenExpiryPrefsKey => $"GSHEETS_TOKEN_EXPIRY";

        private static string SuccessHtml(string code) => $"<html><body><h3>OAuth Success({code})</h3><p>You can now close this tab and go back to Unity.</p></body></html>";
        private static string FailedHtml(string error) => $"<html><body><h3>OAuth Failed</h3><p>Error: {error}</p></body></html>";
        
        [Title("ClientInfo")]
        [SerializeField, TextArea] private string memo;
        [SerializeField, TextArea] private string clientId = "";
        [SerializeField, TextArea] private string clientSecret = "";
        
        [Title("Metadata")]
        [SerializeField, TextArea] private List<string> scopes = new()
        {
            "https://www.googleapis.com/auth/spreadsheets.readonly",
            "https://www.googleapis.com/auth/drive.metadata.readonly",
            "https://www.googleapis.com/auth/drive.readonly",
        };
        [SerializeField] private int port = 45871;

        [Title("Debug")]
        [ShowInInspector, TextArea, ReadOnly] private static string AccessToken
        {
            get => EditorPrefs.GetString(AccessTokenPrefsKey, "");
            set => EditorPrefs.SetString(AccessTokenPrefsKey, value);
        }
        [ShowInInspector, TextArea, ReadOnly] private string RefreshToken
        {
            get => EditorPrefs.GetString(RefreshTokenPrefsKey, "");
            set => EditorPrefs.SetString(RefreshTokenPrefsKey, value);
        }
        [ShowInInspector, TextArea, ReadOnly] private string UpdatedAt
        {
            get => EditorPrefs.GetString(UpdatedAtPrefsKey, "");
            set => EditorPrefs.SetString(UpdatedAtPrefsKey, value);
        }
        [ShowInInspector, TextArea, ReadOnly] private string TokenExpiry
        {
            get => EditorPrefs.GetString(TokenExpiryPrefsKey, "");
            set => EditorPrefs.SetString(AccessTokenPrefsKey, value);
        }

        public static async UniTask<string> GetAccessToken(bool forceRenew = false)
        {
            var instance = EditorInstance;
            if (instance == null)
            {
                var msg = $"{nameof(GoogleOAuthHelper)} 가 없습니다! 생성 후 진행해주세요.";
                Debug.LogError(msg);
                EditorUtility.DisplayDialog("오류", msg, "닫기");
                return null;
            }

            if(!forceRenew)
                return await EditorInstance.GetValidAccessToken();
            else
                return await EditorInstance.TryAuthorize();
        } 
        
        /// <summary>
        /// 저장된 AccessToken을 가져온다.
        /// 토큰이 없거나, 만료되었다면, 새로 가져온다.
        /// </summary>
        private async UniTask<string> GetValidAccessToken()
        {
            // 토큰이 유효하다면, 반환
            var isValid = await ValidateAccessToken(AccessToken);
            if(isValid)
                return AccessToken;

            // 아니라면, 리프래쉬 진행
            AccessToken = await TryRefreshAccessToken();
            if (!string.IsNullOrEmpty(AccessToken))
                return AccessToken;

            // 아니라면, 로그인 진행
            AccessToken = await TryAuthorize();
            if (!string.IsNullOrEmpty(AccessToken))
                return AccessToken;
            
            // 실패
            Debug.LogError("[Auth] 구글 Auth 실패: 알 수 없는 에러", this);
            return null;
        }
        
        /// <summary>
        /// 로그인 후 AccessToken을 가져온다.
        /// </summary>
        private async UniTask<string> TryAuthorize()
        {
            try
            {
                // 1) PKCE 생성
                GeneratePKCE(out var codeVerifier, out var codeChallenge);

                // 2) 로컬 리스너 기동
                using var listener = new HttpListener();
                listener.Prefixes.Add(RedirectUri);
                listener.Start();
                Debug.Log($"[OAuth] Listening: {RedirectUri}");

                // 3) 동의 URL 생성 후 열기
                var url = BuildAuthRequestUrl(codeChallenge);
                Application.OpenURL(url);

                // 4) 콜백 수신(한 번)
                var context = await listener.GetContextAsync().AsUniTask().Timeout(TimeSpan.FromSeconds(30));
                var request = context.Request;

                // 5) code 파싱
                var query = HttpUtility.ParseQueryString(request.Url.Query);
                var code = query.Get("code");
                var error = query.Get("error");

                // 6) 사용자에게 결과를 안내할 페이지 HTML 생성
                var html = string.IsNullOrEmpty(error) ? SuccessHtml(code) : FailedHtml(error);

                // 6-1) 웹에 결과 반환
                var buffer = Encoding.UTF8.GetBytes(html);
                context.Response.ContentLength64 = buffer.Length;
                await context.Response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
                
                // 6-2) 연결 닫기
                context.Response.OutputStream.Close();
                listener.Stop();

                // 에러가 존재한다면 실패 반환
                if (!string.IsNullOrEmpty(error))
                {
                    Debug.LogError("[OAuth] Error: " + error);
                    return null;
                }

                // 코드가 없다면 실패 반환.
                if (string.IsNullOrEmpty(code))
                {
                    Debug.LogError("[OAuth] No code received.");
                    return null;
                }

                // 7) token으로 코드 교환
                AccessToken = await ExchangeCodeForTokens(code, codeVerifier);
                Debug.Log("[OAuth] Token exchange complete.");
                return AccessToken;
            }
            catch (Exception ex)
            {
                Debug.LogError("[OAuth] Exception: " + ex);
                return null;
            }
        }
        
        private async UniTask<string> TryRefreshAccessToken()
        {
            if (string.IsNullOrEmpty(RefreshToken))
            {
                Debug.LogWarning("[Auth] 리프래쉬 토큰이 없습니다.", this);
                return null;
            }

            var form = new WWWForm();
            form.AddField("client_id", clientId);
            form.AddField("client_secret", clientSecret);
            form.AddField("grant_type", "refresh_token");
            form.AddField("refresh_token", RefreshToken);

            using var www = UnityWebRequest.Post(TokenUrl, form);
            await www.SendWebRequest();
            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("[Auth] 리프래쉬 에러: " + www.error + "\n" + www.downloadHandler.text);
                return null;
            }

            var json = www.downloadHandler.text;
            var token = JsonUtility.FromJson<TokenResponse>(json.Trim());
            if (string.IsNullOrEmpty(token.access_token))
            {
                Debug.LogError("[Auth] AccessToken 반환에 실패하였습니다: " + json);
                return null;
            }

            AccessToken = token.access_token;
            
            var expiry = DateTime.UtcNow.AddSeconds(token.expires_in);
            TokenExpiry = expiry.Ticks.ToString();
            
            UpdatedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            Debug.Log("[Auth] AccessToken이 갱신되었습니다.");
            return token.access_token;
        }
        
        private static void GeneratePKCE(out string verifier, out string challenge)
        {
            // verifier: 43~128 chars (unreserved)
            var buffer = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
                rng.GetBytes(buffer);

            // 43 chars around
            verifier = Base64UrlNoPadding(Convert.ToBase64String(buffer)); 
            
            // challenge: BASE64URL-ENCODE(SHA256(verifier))
            using var sha = SHA256.Create();
            var hash = sha.ComputeHash(Encoding.ASCII.GetBytes(verifier));
            challenge = Base64UrlNoPadding(Convert.ToBase64String(hash));
        }
        
        private static string Base64UrlNoPadding(string s) => s.Replace("+", "-").Replace("/", "_").Replace("=", "");

        private string BuildScope() => scopes is { Count: > 0 } ? string.Join(" ", scopes) : "";

        private string BuildAuthRequestUrl(string challenge)
        {
            var query =
                $"client_id={Uri.EscapeDataString(clientId)}" +
                $"&redirect_uri={Uri.EscapeDataString(RedirectUri)}" +
                $"&response_type=code" +
                $"&scope={Uri.EscapeDataString(BuildScope())}" +
                $"&code_challenge={Uri.EscapeDataString(challenge)}" +
                $"&code_challenge_method=S256" +
                $"&access_type=offline" +        // refresh_token 받기
                $"&prompt=consent";              // 첫 동의 강제(없으면 refresh 안 줄 수 있음)

            return $"{AuthUrl}?{query}";
        }
        
        private async UniTask<string> ExchangeCodeForTokens(string code, string codeVerifier)
        {
            var form = new WWWForm();
            form.AddField("client_id", clientId);
            form.AddField("client_secret", clientSecret);
            form.AddField("grant_type", "authorization_code");
            form.AddField("code", code);
            form.AddField("redirect_uri", RedirectUri);
            form.AddField("code_verifier", codeVerifier);

            using var www = UnityWebRequest.Post(TokenUrl, form);
            
#if UNITY_2022_1_OR_NEWER
            www.SetRequestHeader("User-Agent", "UnityEditor");
#endif
            await www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
                throw new Exception("[Token] Error: " + www.error + "\n" + www.downloadHandler.text);

            var json = www.downloadHandler.text;
            var token = JsonUtility.FromJson<TokenResponse>(json?.Trim());

            if (string.IsNullOrEmpty(token.access_token))
                throw new Exception("[Token] No access_token in response: " + json);

            // AccessToken 저장
            AccessToken = token.access_token;
            UpdatedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            // RefreshToken 저장
            if (!string.IsNullOrEmpty(token.refresh_token))
            {
                RefreshToken = token.refresh_token;
            }

            var expiry = DateTime.UtcNow.AddSeconds(token.expires_in);
            TokenExpiry = expiry.Ticks.ToString();

            // 반환
            return token.access_token;
        }
        
        private bool IsTokenExpired()
        {
            if (!long.TryParse(TokenExpiry, out var ticks)) return true;
            
            var expiry = new DateTime(ticks, DateTimeKind.Utc);
            return DateTime.UtcNow >= expiry.AddSeconds(-60); // 60초 여유
        }

        private async UniTask<bool> ValidateAccessToken(string accessToken)
        {
            if (string.IsNullOrWhiteSpace(accessToken)) return false;

            try
            {
                // access_token은 URL 인코딩 권장
                var url = TokenInfoUrl + UnityWebRequest.EscapeURL(accessToken);

                // 리퀘스트
                using var req = UnityWebRequest.Get(url);
                req.timeout = 5;
                await req.SendWebRequest();

                var http = req.responseCode;
                var text = req.downloadHandler?.text;

                // 네트워크 레벨 에러
                if (req.result is UnityWebRequest.Result.ConnectionError or UnityWebRequest.Result.DataProcessingError)
                    throw new Exception(req.error);

                // tokeninfo는 보통:
                // 200: 유효
                // 400: invalid_token / expired_token 등
                if (http != 200)
                    throw new Exception(req.error);

                return true;
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message, this);
                return false;
            }
        }
        
        [Serializable]
        private class TokenResponse
        {
            public string access_token;
            public int    expires_in;
            public string refresh_token;
            public string scope;
            public string token_type;
        }

        [Button("토큰 가져오기")]
        private async void Internal_GetAccessToken()
        {
            Debug.Log($"토큰 가져오기 시도");
            var token = await GetValidAccessToken();
            Debug.Log($"토큰 가져오기 결과: {token}");
        }

        [Button("토큰 비우기")]
        public void Clear()
        {
            AccessToken = string.Empty;
            RefreshToken = string.Empty;
            UpdatedAt = string.Empty;
            TokenExpiry = string.Empty;
        }


        [Button("토큰 유효성 검사", DrawResult = true)]
        private async void Internal_ValidateAccessToken()
        {
            Debug.Log("토큰 유효성 검사 시도: " + AccessToken);
            var result = await ValidateAccessToken(AccessToken);
            Debug.Log("토큰 유효성 검사 결과: " + result);
        }
    }
}
#endif