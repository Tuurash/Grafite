using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorePersonalizationServices;

public static class CustomPrompts
{
   public static string CoreIdentity = @"You are a Child Health Expert.";

   public static string CoreDataResource_txt = ResourceObserver.ReadFullFile(System.IO.Directory.GetCurrentDirectory() + "/tdata_minifiedjson.txt");

   public static string CoreResponsibility = @"From now on, you will always respond in Bangla. However the Topic should be English always. Your response should be in JSON format like the one below,
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
                                                ";

                                                
}
