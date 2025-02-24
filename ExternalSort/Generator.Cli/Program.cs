using Bogus;

namespace Generator.Cli
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var faker = new Faker("en");
            var path = "f:/temp/1gb";

            var size = 1024 * 1024 * 1024;
            using (var file = new StreamWriter(path)) 
            {
                do
                {
                    file.WriteLine($"{faker.Random.Long(1)}. {faker.Random.Words()}");

                } while (file.BaseStream.Length < size);

                file.Flush();
                file.Close();
            }

        }
    }
}
