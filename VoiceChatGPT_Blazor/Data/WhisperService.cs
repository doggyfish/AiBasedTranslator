using Microsoft.AspNetCore.Mvc;
using OpenAI.Interfaces;
using OpenAI.ObjectModels.RequestModels;
using OpenAI.ObjectModels;

namespace VoiceChatGPT_Blazor.Data
{
    public class WhisperService
    {
        private readonly IOpenAIService _openAIService;

        public WhisperService(IOpenAIService openAIService)
        {
            _openAIService = openAIService;
        }

        [HttpPost]
        public async Task<TranslateResponse> TranslateAsync(WhisperRequest request)
        {
            TranslateRequest? translateRequest = null;

            try
            {
                string fileName = request.filename;
                var sampleFile = await System.IO.File.ReadAllBytesAsync($"SampleData/{fileName}");
                var audioResult = await _openAIService.Audio.CreateTranscription(new AudioCreateTranscriptionRequest
                {
                    FileName = fileName,
                    File = sampleFile,
                    Model = Models.WhisperV1,
                    ResponseFormat = StaticValues.AudioStatics.ResponseFormat.VerboseJson
                });
                if (audioResult.Successful)
                {
                    Console.WriteLine(string.Join("\n", audioResult.Text));
                }
                else
                {
                    if (audioResult.Error == null)
                    {
                        throw new Exception("Unknown Error");
                    }
                    var msg = $"{audioResult.Error.Code}: {audioResult.Error.Message}";
                    Console.WriteLine(msg);
                    return new TranslateResponse(
                        "",
                        "",
                        "",
                        msg);
                }

                translateRequest = new TranslateRequest(request.Source, request.Target, audioResult.Text);

                var result = await _openAIService.ChatCompletion.CreateCompletion(new ChatCompletionCreateRequest
                {
                    Model = Models.ChatGpt3_5Turbo,
                    Messages = new List<ChatMessage>
                {
                    ChatMessage.FromSystem($"Translate the following {translateRequest.Source} text to {translateRequest.Target}:"),
                    ChatMessage.FromUser(translateRequest.Text),
                },
                });

                if (!result.Successful)
                {
                    throw new Exception(result.Error?.Message ?? "Translation failed.");
                }

                var response = new TranslateResponse(
                    translateRequest.Source,
                    translateRequest.Target,
                    translateRequest.Text,
                    result.Choices.FirstOrDefault()?.Message.Content ?? "");

                return response;
            }
            catch (Exception ex)
            {
                string msg = ex.GetBaseException().Message;
                TranslateResponse response;
                if (translateRequest == null)
                    response = new TranslateResponse(
                        "",
                        "",
                        "",
                        msg);
                else
                    response = new TranslateResponse(
                        translateRequest.Source,
                        translateRequest.Target,
                        translateRequest.Text,
                        msg);
                return response;
            }
        }
    }
}
