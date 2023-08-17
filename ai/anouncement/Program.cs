using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using Microsoft.CognitiveServices.Speech.Translation;

namespace announcement
{
    internal class Program
    {
        // Based on this quickstart: https://learn.microsoft.com/en-us/azure/ai-services/speech-service/get-started-speech-translation
        // This example requires environment variables named "SPEECH_KEY" and "SPEECH_REGION"
        private static readonly string SpeechKey = Environment.GetEnvironmentVariable("SPEECH_KEY") ?? throw new InvalidOperationException();
        private static readonly string SpeechRegion = Environment.GetEnvironmentVariable("SPEECH_REGION") ?? throw new InvalidOperationException();

        private static void OutputSpeechRecognitionResult(TranslationRecognitionResult translationRecognitionResult)
        {
            switch (translationRecognitionResult.Reason)
            {
                case ResultReason.TranslatedSpeech:
                    Console.WriteLine($"RECOGNIZED: Text={translationRecognitionResult.Text}");
                    foreach (var element in translationRecognitionResult.Translations)
                    {
                        Console.WriteLine($"TRANSLATED into '{element.Key}': {element.Value}");
                    }
                    break;
                case ResultReason.NoMatch:
                    Console.WriteLine($"NOMATCH: Speech could not be recognized.");
                    break;
                case ResultReason.Canceled:
                    var cancellation = CancellationDetails.FromResult(translationRecognitionResult);
                    Console.WriteLine($"CANCELED: Reason={cancellation.Reason}");

                    if (cancellation.Reason == CancellationReason.Error)
                    {
                        Console.WriteLine($"CANCELED: ErrorCode={cancellation.ErrorCode}");
                        Console.WriteLine($"CANCELED: ErrorDetails={cancellation.ErrorDetails}");
                        Console.WriteLine($"CANCELED: Did you set the speech resource key and region values?");
                    }
                    break;
            }
        }

        private static async Task Main()
        {
            var speechTranslationConfig = SpeechTranslationConfig.FromSubscription(SpeechKey, SpeechRegion);
            speechTranslationConfig.SpeechRecognitionLanguage = "en-US";
            speechTranslationConfig.AddTargetLanguage("es");
            speechTranslationConfig.AddTargetLanguage("fr");

            using var audioConfig = AudioConfig.FromDefaultMicrophoneInput();
            using var translationRecognizer = new TranslationRecognizer(speechTranslationConfig, audioConfig);

            Console.WriteLine("Speak into your microphone.");
            var translationRecognitionResult = await translationRecognizer.RecognizeOnceAsync();
            OutputSpeechRecognitionResult(translationRecognitionResult);
        }
    }
}
