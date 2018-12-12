using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace CSPredictionSample
{
    static class Program
    {
        static void Main()
        {
            Console.Write("Enter image file path: ");
            string imageFilePath = Console.ReadLine();

            MakePredictionRequest(imageFilePath).Wait();

            Console.WriteLine("\n\n\nHit ENTER to exit...");
            Console.ReadLine();
        }

        static byte[] GetImageAsByteArray(string imageFilePath)
        {
            FileStream fileStream = new FileStream(imageFilePath, FileMode.Open, FileAccess.Read);
            BinaryReader binaryReader = new BinaryReader(fileStream);
            return binaryReader.ReadBytes((int)fileStream.Length);
        }

        static async Task MakePredictionRequest(string imageFilePath)
        {
            var client = new HttpClient();

            // Request headers - replace this example key with your valid subscription key.
            client.DefaultRequestHeaders.Add("Prediction-Key", "30bb428a529948a483cae7cd9e020c54");

            // Prediction URL - replace this example URL with your valid prediction URL.
            string url = "https://southcentralus.api.cognitive.microsoft.com/customvision/v2.0/Prediction/17c04784-53f3-4fd7-aada-9779aa7902c2/image?iterationId=deb4d9b5-cdd4-4e2f-83cc-d5ec033e7243";

            HttpResponseMessage response;

            // Request body. Try this sample with a locally stored image.
            byte[] byteData = GetImageAsByteArray(imageFilePath);
            try
            {
                using (var content = new ByteArrayContent(byteData))
                {
                    content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                    response = await client.PostAsync(url, content);
                    var result = await response.Content.ReadAsStringAsync();
                    JObject json = JObject.Parse(result);

                    int prediccionnes = ((JArray)json["predictions"]).Count;

                    ////// Loop over each prediction and write out the results
                    //foreach (var c in json.Predictions)
                    //{
                    //    Console.WriteLine($"\t{c.TagName}: {c.Probability:P1} [ {c.BoundingBox.Left}, {c.BoundingBox.Top}, {c.BoundingBox.Width}, {c.BoundingBox.Height} ]");
                    //}

                    for (int i = 0; i < prediccionnes; i++)
                    {
                        if ((double)json["predictions"][i]["probability"] > 0.4)
                        {
                            Console.WriteLine("La predicción N° " + i);
                            Console.WriteLine("Probabilidad: "+ (string)json["predictions"][i]["probability"]);
                            Console.WriteLine("Left: " + (string)json["predictions"][i]["boundingBox"]["left"]);
                            Console.WriteLine("Top: " + (string)json["predictions"][i]["boundingBox"]["top"]);
                            Console.WriteLine("Width: " + (string)json["predictions"][i]["boundingBox"]["width"]);
                            Console.WriteLine("height: " + (string)json["predictions"][i]["boundingBox"]["height"]);
                        } 
                    }

                    
                }
            }
            catch (Exception error) {
                Console.WriteLine(error.Message);
            }
        }
    }
}
