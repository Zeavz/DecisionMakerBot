using Discord;
using Discord.Commands;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RandomBot
{
    class MyBot
    {
        DiscordClient discord;
        List<string> list = new List<string>();
        int choices;
        Boolean running = false;

        public MyBot()
        {
            discord = new DiscordClient(x =>
            {
                x.LogLevel = LogSeverity.Info;
                x.LogHandler = Log;
            });

            discord.UsingCommands(x =>
            {
                x.PrefixChar = '!';
                x.AllowMentionPrefix = true;

            });

            var commands = discord.GetService<CommandService>();
            commands.CreateCommand("rand").Do(async (e) =>
            {
                await e.Channel.SendMessage("use #[option] to add an option");
                list.Clear();
                running = true;
            });

            commands.CreateCommand("end").Do(async (e) =>
            {
                if (running)
                {
                    string chosenChoice;
                    if (!list.Any())
                    {
                        chosenChoice = "You must enter options for me to chose from";
                    }
                    else
                    {
                        Random rnd = new Random();
                        int choice = rnd.Next(0, choices);
                        chosenChoice = list[choice];
                        list.Clear();
                    }
                    running = false;
                    await e.Channel.SendMessage(chosenChoice);
                }
                else
                {
                    await e.Channel.SendMessage("Start the command with !rand");
                }
            });

            discord.MessageReceived += async (s, e) =>
            {
                if (running)
                {
                    string message = e.Message.ToString();
                    int counter = 0;
                    if (message.Contains('#') && !e.Message.IsAuthor)
                    {
                        while (!message[counter].Equals('#'))
                        {
                            counter++;
                        }
                        string newMessage = message.Substring(counter + 1);

                        if (newMessage.Length < 1)
                        {
                            await e.Channel.SendMessage("Empty option inserted, was not added to list");
                        }
                        else
                        {
                            list.Add(newMessage);
                            choices++;
                        }
                    }
                }
            };

            discord.ExecuteAndWait(async () =>
            {
                while (true)
                {
                    try
                    {
                        await discord.Connect("INSERT_YOUR_TOKEN_HERE", TokenType.Bot);
                        break;
                    }
                    catch
                    {
                        await Task.Delay(3000);
                    }
                }
            });
        }

        private void Log(object sender, LogMessageEventArgs e)
        {
            Console.WriteLine(e.Message);
        }
    }
}
