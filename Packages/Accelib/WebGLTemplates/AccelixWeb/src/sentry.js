import * as Sentry from "@sentry/browser";

Sentry.init({
  dsn: "https://cba71630b07f0f9fc21846a65fc431f1@o4509444614651904.ingest.us.sentry.io/4509467798667264",
  sendDefaultPii: true,
  sampleRate: 0.5, // Adjust the sample rate as needed
  integrations: [
    Sentry.contextLinesIntegration(),
    Sentry.extraErrorDataIntegration()
  ],
});

window.onerror = function (message, source, lineno, colno, error) {
  Sentry.captureException(error || new Error(message));
};
window.onunhandledrejection = function (event) {
  Sentry.captureException(event.reason || new Error("Unhandled promise rejection"));
};