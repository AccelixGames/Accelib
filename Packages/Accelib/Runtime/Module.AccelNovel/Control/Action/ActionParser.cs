using System.Collections.Generic;
using System.Text.RegularExpressions;
using Accelib.Logging;
using Accelib.Module.AccelNovel.Model;
using Accelib.Module.AccelNovel.Model.SO;
using Accelix.Accelib.AccelNovel.Model;
using AYellowpaper.SerializedCollections;

namespace Accelib.Module.AccelNovel.Control.Action
{
    public static class ActionParser
    {
        private const string ChunkPattern = @"(\[[^\]]*\])|([^\[]+)";

        public static List<ActionLine> ParseActions(SO_Scenario scenario)
        {
            // 액션라인 생성
            var actionLines = new List<ActionLine>();
            
            // 스크립트 라인 순회하며,
            foreach (var scriptLine in scenario.ScriptLines)
            {
                // 청크 분할
                var chunks = ChunkScript(scriptLine.text);
                
                // 청크를 순회하며, 패턴분할
                foreach (var baseChunk in chunks)
                {   
                    // 액션라인 생성
                    var actionLine = new ActionLine
                    {
                        label = scriptLine.label,
                        characterKey = scriptLine.characterKey,
                        voiceKey = scriptLine.voiceKey
                    };

                    // 청크 캐싱
                    if (!baseChunk.StartsWith("[") && !baseChunk.EndsWith("]"))
                    {
                        actionLine.key = "dlg";
                        actionLine.value = baseChunk;
                    }
                    else
                    {
                        var chunk = baseChunk[1..^1];
                        var keyValuePairs = chunk.Split(',');
                        
                        // 청크 파싱 및 순회
                        for (var i = 0; i < keyValuePairs.Length; i++)
                        {
                            var kv = keyValuePairs[i].Split('=');
                            if (kv.Length != 2) continue;

                            var key = kv[0].Trim();
                            var value = kv[1].Trim();
                            if (i == 0)
                            {
                                actionLine.key = key;
                                actionLine.value = value;
                            }
                            else
                            {
                                actionLine.arguments ??= new SerializedDictionary<string, string>();
                                if(!actionLine.arguments.TryAdd(key, value))
                                    Deb.LogError($"중복된 키입니다. Key[{key}]Value[{value}] - ({chunk})", scenario);
                            }
                        }
                    }

                    actionLines.Add(actionLine);
                }
            }

            // Maid
            if (scenario.Dialogues?.Count > 0)
            {
                var idx = 0;
                for (int i = 0; i < actionLines.Count; ++i)
                {
                    if (actionLines[i].key == "dlg")
                    {
                        actionLines[i].stateName = scenario.Dialogues[idx].stateName;
                        actionLines[i].stateChange = scenario.Dialogues[idx].stateChange;
                        idx++;
                    }
                }
            }
            
            // 반환
            return actionLines;
        }
        
        private static List<string> ChunkScript(string input)
        {
            var chunks = new List<string>();
            var matches = Regex.Matches(input, ChunkPattern);
            var normalText = "";

            foreach (Match match in matches)
            {
                if (match.Groups[1].Success) // 대괄호로 묶인 경우
                {
                    if (!string.IsNullOrWhiteSpace(normalText))
                    {
                        chunks.Add(normalText.Trim());
                        normalText = "";
                    }
                    chunks.Add(match.Groups[1].Value.Trim());
                }
                else if (match.Groups[2].Success) // 일반 텍스트
                {
                    normalText += match.Groups[2].Value;
                }
            }

            if (!string.IsNullOrWhiteSpace(normalText))
            {
                chunks.Add(normalText.Trim());
            }

            return chunks;
        }
        
        // private static List<string> ChunkScript(string input)
        // {
        //     var chunks = new List<string>();
        //     var matches = Regex.Matches(input, ChunkPattern);
        //
        //     foreach (Match match in matches)
        //     {
        //         var chunk = match.Groups[1].Success ? match.Groups[1].Value : match.Groups[2].Value;
        //         chunks.Add(chunk.Trim());
        //     }
        //
        //     return chunks;
        // }

        // public static List<ActionLine> ParseActions(SO_Scenario scenario)
        // {
        //     var actionLines = new List<ActionLine>();
        //     
        //     foreach (var line in scenario.ScriptLines)
        //     {
        //         var dialogue = "";
        //         
        //         var actionLine = new ActionLine
        //         {
        //             label = line.label,
        //             characterKey = line.characterKey
        //         };
        //         
        //         var trimmed = line.text.Trim();
        //     
        //         // 괄호 안에 있는 key=value 파싱
        //         var matches = Regex.Matches(trimmed, Pattern);
        //         foreach (Match match in matches)
        //         {
        //             var content = match.Groups[1].Value;
        //             var keyValuePairs = content.Split(',');
        //             var isFirstPair = true;
        //         
        //             foreach (var pair in keyValuePairs)
        //             {
        //                 var kv = pair.Split('=');
        //                 if (kv.Length == 2)
        //                 {
        //                     var parsedPair = new ActionLine.Pair { key = kv[0].Trim(), value =  kv[1].Trim()};
        //                     if (isFirstPair)
        //                     {
        //                         actionLine.key = parsedPair.key;
        //                         actionLine.value = parsedPair.value;
        //                         isFirstPair = false;
        //                     }
        //                     else
        //                     {
        //                         actionLine.arguments ??= new List<ActionLine.Pair>();
        //                         actionLine.arguments.Add(parsedPair);
        //                     }
        //                 }
        //             }
        //             
        //             actionLines.Add(actionLine);
        //         }
        //     
        //         // 괄호가 없는 경우 dlg에 저장
        //         if (!trimmed.StartsWith("[") && !trimmed.EndsWith("]"))
        //         {
        //             if (dialogue.Length > 0) dialogue += " ";
        //             dialogue += trimmed;
        //         }
        //         
        //         if (!string.IsNullOrEmpty(dialogue))
        //         {
        //             actionLine.key = "dlg";
        //             actionLine.value = dialogue;
        //             actionLines.Add(actionLine);
        //         }
        //     }
        //     
        //     return actionLines;
        // }
    }
}