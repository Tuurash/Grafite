using ChildHealthBot.Models;

using ChildHealthBot.Rsc;
using ChildHealthBot.Services;

using Newtonsoft.Json;

using System.Text;
using System.Text.RegularExpressions;

internal class Program
{
   private static BardServices BardServices;
   private static PromptModel SessionPrompt;

   private static bool IsNewConversation = true;

   private static void Main(string[] args)
   {
      Console.OutputEncoding = System.Text.Encoding.UTF8;

      Task t = MainAsync(args);
      t.Wait();
   }

   private static async Task MainAsync(string[] args)
   {
      await InitializeNewChat();
   }

   private static async Task InitializeNewChat()
   {
      Console.Clear();
      Console.WriteLine("           Starting New Conversation          \n");

      if (IsNewConversation)
      {
         SessionPrompt = new PromptModel();

         BardServices = new BardServices();

         IsNewConversation = false;

         await InitializeBot().ConfigureAwait(true);

         await StartChat();
      }
   }

   private static async Task InitializeBot()
   {
      bool IsBotReady = false;

      string p1 = @"I will give you a minified JSON data. please read and memorize fully";
      string p2 = ReadFullFile(System.IO.Directory.GetCurrentDirectory() + "/tdata_minifiedjson.txt") + "\n Reply \"DONE\" if you have fully read and memorized";
      string p3 = @"From now on, you will always respond in Bangla. However the Topic should be English always. Your response should be in JSON format like the one below,
                                                {
                                                'Topic': 'This will be in English',
                                                'Context': [
                                                'বাচ্চার বয়স ১০ বছর। এই সমস্যা তিনি এক মাস ধরে অনুভব করছে।',
                                                'মেয়ের বয়স ১২ বছর। এই সমস্যা তিনি এক মাস ধরে বুঝতে পাচ্ছে।',
                                                'বাচ্চার বয়স ৯ বছর। এই সমস্যা দুই মাস ধরে চলছে।'
                                                ],
                                                'Answer': [
                                                'your answer in Bangla'
                                                ]
                                                }

                                                the minified JSON data is for your answer reference. whenever you recognize any similar symptom or questions, you should respond based on the json daata information. And you will follow the topic and contexts. Reply 'DONE' if you have fully read and memorized.";

      try
      {
         //While Enter is Pressed Continue
         while (!IsBotReady)
         {
            //p1
            string Query = p1;

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

            var response = await BardServices.GetResponse(SessionPrompt).ConfigureAwait(false);

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
            Query = p2;

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

            response = await BardServices.GetResponse(SessionPrompt).ConfigureAwait(true);

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
            Query = p3;

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

            response = await BardServices.GetResponse(SessionPrompt).ConfigureAwait(true);

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

   private static async Task StartChat()
   {
      try
      {
         //While Enter is Pressed Continue
         while (!IsNewConversation)
         {
            Console.Write(@"your query:");
            string Query = Console.ReadLine() ?? "";

            if (Query == "new")
            {
               IsNewConversation = true;
               await InitializeNewChat().ConfigureAwait(true);
               continue;
            }

            if (!string.IsNullOrEmpty(Query))
            {
               SessionPrompt.Contents.Add(new PromptContent()
               {
                  Role = "user",
                  Parts =
                   [
                       new() { Text = Query }
                   ],
               });
            }

            Console.WriteLine("\n Thinking ...\n");

            var response = await BardServices.GetResponse(SessionPrompt).ConfigureAwait(true);

            if (!string.IsNullOrEmpty(response))
            {
               if (response != "Error" || response != "Exception")
               {
                  SessionPrompt.Contents.Add(new PromptContent()
                  {
                     Role = "model",
                     Parts =
                      [
                          new() { Text = response }
                      ],
                  });
               }
               //else { SessionPrompt.Contents.RemoveAt(-1); }

               Console.WriteLine("\n Model: " + response + "\n");
            }
         }//End
      }
      catch (Exception ex)
      {
         Console.WriteLine(ex.Message);
      }
   }

   public static string ReadFullFile(string path)
   {
      //get current path
      string C = System.IO.Directory.GetCurrentDirectory();

      string file = System.IO.File.ReadAllText(path);
      return file;
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





}