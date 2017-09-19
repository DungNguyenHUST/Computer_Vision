using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;

namespace CP_Test
{
    static class Program
    {
        const string subscriptionKey = "cbbd2e8a6d1049a0b23b56294c5fb0e6";
        const string uriBase = "https://westcentralus.api.cognitive.microsoft.com/vision/v1.0/analyze";

        static void Main(string[] args)
        {
            Console.WriteLine("Analyze an image : ");
            Console.Write("Enter the path to an image you wish to analyze:");
            string imageFilePath = Console.ReadLine();

            MakeAnalysisRequest(imageFilePath);
            Console.WriteLine("\n Please wait a moment for the result to appear. Then, press Enter to exit \n");
            Console.ReadLine();

            //Console.WriteLine("Enter an ID for the group you wish to create:");
            //Console.WriteLine("(Use numbers, lower case letters, '-' and '_'. The maximum length of the personGroupId is 64.)");
            //string personGroupId = Console.ReadLine();
            //MakeCreateGroupRequest(personGroupId);
            //Console.WriteLine("\n\n\nWait for the result below, then hit ENTER to exit...\n\n\n");
            //Console.ReadLine();
        }
        static async void MakeAnalysisRequest(string imageFilePath)
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", subscriptionKey);
            string requestParameters = "visualFeatures=Categories,Description,Color&language=en";
            string uri = uriBase + "?" + requestParameters;
            HttpResponseMessage response;
            byte[] byteData = GetImageAsByteArray(imageFilePath);
            using (ByteArrayContent content = new ByteArrayContent(byteData))
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                response = await client.PostAsync(uri, content);
                string contentString = await response.Content.ReadAsStringAsync();
                Console.WriteLine("\nResponse :\n");
                Console.WriteLine(JsonPrettyPrint(contentString));
                string savePath = @"C:\CV.txt";
               // string fileName = @"CV.txt";
                System.IO.File.WriteAllText(savePath, contentString);
            }
        }
        static byte[] GetImageAsByteArray(string imageFilePath)
        {
            FileStream fileStream = new FileStream(imageFilePath, FileMode.Open, FileAccess.Read);
            BinaryReader binaryReader = new BinaryReader(fileStream);
            return binaryReader.ReadBytes((int)fileStream.Length);
        }
        static string JsonPrettyPrint(string json)
        {
            if (string.IsNullOrEmpty(json))
                return string.Empty;
            json = json.Replace(Environment.NewLine, "").Replace("t", "");
            StringBuilder sb = new StringBuilder();
            bool quote = false;
            bool ignore = false;
            int offset = 0;
            int indentLength = 3;
            foreach(char ch in json)
            {
                switch(ch)
                {
                    case '"':
                        if (!ignore) quote = !quote;
                        break;
                    case '\'':
                        if (quote) ignore = !ignore;
                        break;
                }
                if (quote)
                    sb.Append(ch);
                else
                {
                    switch(ch)
                    {
                        case '{':
                        case '[':
                            sb.Append(ch);
                            sb.Append(Environment.NewLine);
                            sb.Append(new string(' ', ++offset * indentLength));
                            break;
                        case '}':
                        case ']':
                        case ',':
                            sb.Append(Environment.NewLine);
                            sb.Append(new string(' ', offset * indentLength));
                            break;
                        case ':':
                            sb.Append(ch);
                            sb.Append(' ');
                            break;
                        default:
                            if (ch != ' ') sb.Append(ch);
                            break;
                    }
                }
            }
            return sb.ToString().Trim();
        }
        static async void MakeCreateGroupRequest(string personGroupId)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "956fe35ea7534b51b57f8b32fd454a2b");
            string uri = "https://westcentralus.api.cognitive.microsoft.com/face/v1.0/persongroups/" + personGroupId;
            string json = "{\"name\":\"My Group\", \"userData\":\"Some data related to my group.\"}";
            HttpContent content = new StringContent(json);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            HttpResponseMessage response = await client.PutAsync(uri, content);
            Console.WriteLine("Response status : " + response.StatusCode);
        }
    }
}
