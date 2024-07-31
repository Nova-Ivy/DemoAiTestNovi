using Azure.AI.Translation.Text;
using Azure;
using DemoAiTestNovi.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Azure.AI.OpenAI;
using OpenAI.Chat;
using System.Transactions;
using System.Text;
 
namespace DemoAiTestNovi.Controllers

{

        public class HomeController : Controller

        {

            private readonly ILogger<HomeController> _logger;

            string aiEndpoint = "https://aiservice2405.openai.azure.com/";

            string key = "c265f353cd4f4d10becad430e15fc4f6";

            string region = "swedencentral";

            public HomeController(ILogger<HomeController> logger)

            {

                _logger = logger;

            }

            public IActionResult Index()

            {

                return View();

            }



            public async Task<IActionResult> Translate(string srcText, string language)

            {

                var client = new TextTranslationClient(new AzureKeyCredential(key), region);

                //Console.WriteLine("Text to translate:");

                string sourceText = srcText;

                string targetLanguage = language;

                var response = await client.TranslateAsync(targetLanguage, sourceText);

                TranslatedTextItem translation = response.Value.FirstOrDefault();

                //Console.WriteLine($"Source language: {translation?.DetectedLanguage?.Language}");

                //Console.WriteLine($"Translated text: '{translation?.Translations?.FirstOrDefault()?.Text}'.");

                return Json(new { translatedText = translation?.Translations?.FirstOrDefault()?.Text });

            }

            public async Task<IActionResult> Chat(string mychat)

            {

                if (string.IsNullOrEmpty(mychat))

                {

                    return BadRequest("Chat input cannot be empty.");

                }

                var azureAIClient = new AzureOpenAIClient(new Uri(aiEndpoint), key);

                var chatClient = azureAIClient.GetChatClient("gpt-4o");

                // Ensure this model name is correct

                var completionUpdates = chatClient.CompleteChatStreamingAsync

                ([

                        new SystemChatMessage("You are a helpful AI assistant."),

                    new UserChatMessage(mychat)

                  ]);

                var fullResponse = new StringBuilder();

                await foreach (var update in completionUpdates)

                {

                    foreach (var contentPart in update.ContentUpdate)

                    {

                        fullResponse.Append(contentPart.Text);

                    }

                }

                if (fullResponse.Length == 0)

                {

                    return Json(new { translatedText = "No response received." });

                }

                return Json(new { translatedText = fullResponse.ToString() });

            }


            public IActionResult Privacy()

            {

                return View();

            }

            [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]

            public IActionResult Error()

            {

                return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });

            }

        }

    }


