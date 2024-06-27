using CorePersonalizationServices;

using GlobalModels;

using Newtonsoft.Json;

using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;

namespace GeminiCore;

public class PersonalizedGemini
{
   private GeminiCoreService coreService;

   public PromptModel SessionPrompt { get; set; } = new PromptModel();

   string ErrorResponse = @"Something went wrong. Please refresh";

   public PersonalizedGemini()
   {
      coreService = new GeminiCoreService();
   }


   public string SetCustomErrorRespose(string message)
   {
      if (!string.IsNullOrEmpty(message))
      {
         ErrorResponse = message;
      }

      return ErrorResponse;
   }

   //public async Task StartChat(bool IsNewSession)
   //{
   //   try
   //   {
   //      //While Enter is Pressed Continue
   //      while (!IsNewSession)
   //      {
   //         Console.Write(@"your query:");
   //         string Query = Console.ReadLine() ?? "";

   //         if (Query == "new")
   //         {
   //            IsNewSession = true;
   //            await InitializeNewChat().ConfigureAwait(true);
   //            continue;
   //         }

   //         if (!string.IsNullOrEmpty(Query))
   //         {
   //            SessionPrompt.Contents.Add(new PromptContent()
   //            {
   //               Role = "user",
   //               Parts =
   //                [
   //                    new() { Text = Query }
   //                ],
   //            });
   //         }

   //         Console.WriteLine("\n Thinking ...\n");

   //         var response = await coreService.GetResponse(SessionPrompt).ConfigureAwait(true);

   //         if (!string.IsNullOrEmpty(response))
   //         {
   //            if (response != "Error" || response != "Exception")
   //            {
   //               SessionPrompt.Contents.Add(new PromptContent()
   //               {
   //                  Role = "model",
   //                  Parts =
   //                   [
   //                       new() { Text = response }
   //                   ],
   //               });
   //            }
   //            //else { SessionPrompt.Contents.RemoveAt(-1); }

   //            Console.WriteLine("\n Model: " + response + "\n");
   //         }
   //      }//End
   //   }
   //   catch (Exception ex)
   //   {
   //      Console.WriteLine(ex.Message);
   //   }
   //}

   //Modified

   int FailInitializationCount = 0;

   public async Task InitializeNewChat()
   {
      if (FailInitializationCount > 3)
      {
         Debug.WriteLine("           Initialization Failed          \n");
      }
      else
      {

         try
         {

            bool IsNewSession = true;
            Debug.WriteLine("           Starting New Conversation          \n");

            if (IsNewSession)
            {
               SessionPrompt = new PromptModel();

               coreService = new GeminiCoreService();

               IsNewSession = false;

               await InitializeBot().ConfigureAwait(true);

               FailInitializationCount = 0;

               Debug.WriteLine(" \n \n           Initialization Success          \n \n");

            }
         }
         catch (Exception ex)
         {
            FailInitializationCount++;
            Debug.WriteLine(ex.Message);
         }


      }
   }

   public async Task InitializeBot()
   {
      bool IsBotReady = false;


      try
      {
         //While Enter is Pressed Continue
         while (!IsBotReady)
         {
            //p1
            string Query = CustomPrompts.CoreIdentity + "\n If you understand your role. just response with one word READY.";

            if (Query == "ready")
            {
               IsBotReady = true;
               continue;
            }

            if (!string.IsNullOrEmpty(Query))
            {
               SessionPrompt.Contents.Add(new PromptContent()
               {
                  Role = "user",
                  Parts =
                   [
                       new() { Text=Query }
                   ],
               });
            }

            //Console.WriteLine("\n Thinking ...\n");

            var response = await coreService.GetResponse(SessionPrompt).ConfigureAwait(false);

            if (!string.IsNullOrEmpty(response))
            {
               if (response != "Error" || response != "Exception")
               {
                  SessionPrompt.Contents.Add(new PromptContent()
                  {
                     Role = "model",
                     Parts =
                      [
                          new() { Text=response }
                      ],
                  });
               }
               //else { SessionPrompt.Contents.RemoveAt(-1); }

               //Console.WriteLine("\n Model: " + Response + "\n");
            }

            //p2
            Query = "Below is a reference primary data. While response, You should Prioritize these reference as primary resouce of information. \n\n " + CustomPrompts.CoreDataResource_txt + "\n If you understand this instruction. just response with one word READY.";

            if (Query == "ready")
            {
               IsBotReady = true;
               continue;
            }

            if (!string.IsNullOrEmpty(Query))
            {
               SessionPrompt.Contents.Add(new PromptContent()
               {
                  Role = "user",
                  Parts =
                   [
                       new() { Text=Query }
                   ],
               });
            }

            //Console.WriteLine("\n Thinking ...\n");

            response = await coreService.GetResponse(SessionPrompt).ConfigureAwait(true);

            if (!string.IsNullOrEmpty(response))
            {
               if (response != "Error" || response != "Exception")
               {
                  SessionPrompt.Contents.Add(new PromptContent()
                  {
                     Role = "model",
                     Parts =
                      [
                          new() { Text=response }
                      ],
                  });
               }
               //else { SessionPrompt.Contents.RemoveAt(-1); }

               //Console.WriteLine("\n Model: " + Response + "\n");
            }

            //p3
            Query = CustomPrompts.CoreResponsibility + "\n If you understand this instruction. just response with one word READY.";

            if (Query == "ready")
            {
               Console.WriteLine("\n READY \n");
               IsBotReady = true;
               continue;
            }

            if (!string.IsNullOrEmpty(Query))
            {
               SessionPrompt.Contents.Add(new PromptContent()
               {
                  Role = "user",
                  Parts =
                   [
                       new() { Text=Query }
                   ],
               });
            }

            Console.WriteLine("\n Thinking ...\n");

            response = await coreService.GetResponse(SessionPrompt).ConfigureAwait(true);

            if (!string.IsNullOrEmpty(response))
            {
               if (response != "Error" || response != "Exception")
               {
                  SessionPrompt.Contents.Add(new PromptContent()
                  {
                     Role = "model",
                     Parts =
                      [
                          new() { Text=response }
                      ],
                  });
               }

               //Convert SessionPrompt to json
               string json = JsonConvert.SerializeObject(SessionPrompt);

               //else { SessionPrompt.Contents.RemoveAt(-1); }

               //Console.WriteLine("\n Model: " + Response + "\n");

               //try parsing response
               try
               {
                  string extractedContent = ExtractBetweenMarkers(response, "```");
                  // Convert the string content to bytes using UTF-8 encoding
                  byte[] jsonBytes = Encoding.UTF8.GetBytes(extractedContent);

                  // Deserialize the bytes to the desired object
                  var formattedResponse = JsonConvert.DeserializeObject<JsonFormatAnsewer>(Encoding.UTF8.GetString(jsonBytes));
                  IsBotReady = true;
               }
               catch (Exception ex)
               {
                  Console.WriteLine(ex.Message);
               }
            }
         }//End
      }
      catch (Exception ex)
      {
         Console.WriteLine(ex.Message);
      }
   }

   private async Task<string> GetResponseFromCoreService()
   {
      return await coreService.GetResponse(SessionPrompt).ConfigureAwait(true);
   }

   public async Task<string> AddQueryToSession(string query)
   {
      string _response = String.Empty;

      try
      {
         if (!string.IsNullOrEmpty(query))
         {
            SessionPrompt.Contents.Add(new PromptContent()
            {
               Role = "user",
               Parts =
                [
                    new() { Text = query }
                ],
            });
         }


         Debug.WriteLine("\n Thinking ...\n");

         _response = await GetResponseFromCoreService();

      }
      catch (Exception ex)
      {
         _response = ErrorResponse;
         Debug.WriteLine(ex.Message);
      }

      return _response;

   }

   private async Task<string> GetResponseFCSAsync()
   {
      string _response = string.Empty;

      try
      {
         _response = await coreService.GetResponse(SessionPrompt).ConfigureAwait(true);

         if (!string.IsNullOrEmpty(_response))
         {
            await HandleCSResponseAsync(_response).ConfigureAwait(false);
         }
         else
         {
            _response = ErrorResponse;
         }
      }
      catch { _response = ErrorResponse; }

      return _response;
   }

   private async Task HandleCSResponseAsync(string response = "")
   {
      await Task.Run(() =>
      {
         try
         {

            if (!string.IsNullOrEmpty(response) && response != "Error" && response != "Exception")
            {

               Debug.WriteLine("\n Model: " + response + "\n");

               SessionPrompt.Contents.Add(new PromptContent()
               {
                  Role = "model",
                  Parts =
                   [
                       new() { Text = response }
                   ],
               });


            }
            else
            {
               SessionPrompt.Contents.RemoveAt(-1);

            }
         }
         catch (Exception ex)
         {
            Debug.WriteLine("\n\n" + ex.Message + "\n\n");

         }
      });
   }

   private static string ExtractBetweenMarkers(string input, string marker)
   {
      string pattern = $@"{marker}(.*?){marker}";

      Match match = Regex.Match(input, pattern, RegexOptions.Singleline);

      if (match.Success)
      {
         return match.Groups[1].Value;
      }

      return string.Empty;
   }

   public async Task<PromptModel> EndUserSessionAsync()
   {
      PromptModel CurrentSession = new PromptModel();

      try
      {
         if (SessionPrompt != null) CurrentSession = SessionPrompt;
      }
      catch (Exception ex)
      {
         Debug.WriteLine(ex.Message);
      }
      finally
      {
         SessionPrompt = new PromptModel();
      }

      await Task.Delay(500);
      return CurrentSession;
   }

}


