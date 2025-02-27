using Bogus;

namespace Generator.Cli
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var faker = new Faker("en");
            var path = "f:/temp/1v1gb";
            var gigabyte = 1024 * 1024 * 1024;
            long size = gigabyte;//(long)gigabyte * 10;
            using (var file = new StreamWriter(path)) 
            {
                do
                {
                    await file.WriteLineAsync($"{faker.Random.Long(1)}. {faker.Random.Words(10)}");

                } while (file.BaseStream.Length < size);

                file.Flush();
                file.Close();
            }

        }
    }
}
