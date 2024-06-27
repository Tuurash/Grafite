using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChildHealthBot.Rsc;

public static class DefaultPrompt
{
    public static string ReadFullFile(string path)
    {
        //get current path
        string C = System.IO.Directory.GetCurrentDirectory();

        string file = System.IO.File.ReadAllText(path);
        return file;
    }


    public static string DefaultPromptString1 = @"I will give you a minified JSON file. please read and memorize fully";
    public static string DefaultPromptString2 = ReadFullFile(System.IO.Directory.GetCurrentDirectory() + "/tdata_minifiedjson.txt") + "\n Reply \"DONE\" if you have fully read and memorized";
    public static string DefaultPromptString3 = @"From now on, you will always respond in Bangla. However the Topic should be English always. Your response should be in JSON format like the one below,
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

}
