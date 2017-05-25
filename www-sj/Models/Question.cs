using System;
using System.IO;
using Newtonsoft.Json;

namespace www_sj.Models
{
    public class Question
    {
        public int OrdinalNumber { get; set; }
        public string Author { get; set; }
        public string Text { get; set; }
        public string Image { get; set; }
        public string Answer { get; set; }
        public string AnswerImage { get; set; }
        public bool Asked { get; set; }

        public void Save(string folder, string target)
        {
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
            var imageFileName = $"{Guid.NewGuid()}.{Path.GetExtension(Image)}";
            var imageFilePath = folder + imageFileName;
            File.Copy(Image, imageFilePath, true);
            var question = new Question
            {
                Image = imageFilePath,
                Text = Text
            };
            var questionTarget = folder + target;
            using (var outputFile = new StreamWriter(questionTarget))
            {
                outputFile.Write(JsonConvert.SerializeObject(question));
            }
        }
    }
}
