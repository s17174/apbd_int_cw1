using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Cw1
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            try
            {
                if (args.Length < 1)
                {
                    throw new ArgumentNullException();
                }
                else
                {
                    await GetEmailsFromWebsite(args[0]);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Zgłoszono wyjątek: \t{e}");
            }
        }


        public static async Task GetEmailsFromWebsite(String url)
        {
            bool htmlCheck = Uri.TryCreate(url, UriKind.Absolute, out Uri uriResult)
                && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);

            if (!htmlCheck)
            {
                throw new ArgumentException();
            }
            else
            {
                Console.WriteLine("Poprawny URL");
                try
                {
                    var httpClient = new HttpClient();

                    var response = await httpClient.GetAsync(url);

                    if (response.IsSuccessStatusCode)
                    {
                        var htmlContent = await response.Content.ReadAsStringAsync();

                        var regex = new Regex("[a-z]+[a-z0-9]*@[a-z0-9]+\\.[a-z]+", RegexOptions.IgnoreCase);

                        var matches = regex.Matches(htmlContent);
                        if (matches.Count == 0)
                        {
                            Console.WriteLine("Nie znaleziono adresów e-mail");
                        }
                        else
                        {

                            foreach (var i in GetUniqueEmails(matches))
                            {
                                Console.WriteLine(i.ToString());
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("Błąd w czasie pobierania strony");
                    }
                    httpClient.Dispose();


                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Wystąpił błąd {ex}");
                }
                Console.WriteLine("Koniec działania programu!");
            }
        }

        private static IEnumerable<string> GetUniqueEmails(MatchCollection matches)
        {
            return matches
                .OfType<Match>()
                .Select(m => m.Value)
                .Distinct();
        }
    }

}
