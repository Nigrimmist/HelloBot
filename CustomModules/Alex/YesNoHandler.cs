using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HelloBotCommunication;
using HelloBotCommunication.ClientDataInterfaces;


namespace SmartAssHandlerLib
{
    public class YesNoHandler : IActionHandler
    {
        private readonly RandomHelper _decisionMaker = new RandomHelper();

        private SpecialAnswersProvider _specialAnswersProvider;
        private GeneralAnswersProvider _generalAnswersProvider;
        private EmptyAnswerProvider _emptyAnswerProvider;

        private const int CHANCE_OF_SPECIAL_ANSWER = 30;

        public YesNoHandler()
        {
            _specialAnswersProvider = new SpecialAnswersProvider(_decisionMaker);
            _generalAnswersProvider = new GeneralAnswersProvider(_decisionMaker);
            _emptyAnswerProvider = new EmptyAnswerProvider(_decisionMaker);
        }

        public List<string> CallCommandList
        {
            get { return new List<string>() { "бот" }; }
        }

        public string CommandDescription
        {
            get { return "Лаконичный ответ на простой вопрос."; }
        }

        public void HandleMessage(string args, object clientData, Action<string> sendMessageFunc)
        {
            var answer = string.Empty;

            string fromSkypeName = string.Empty;

            var skypeData = clientData as ISkypeData;
            if (skypeData != null)
            {
                fromSkypeName = skypeData.FromName;
            }

            if (!string.IsNullOrEmpty(args) && args.Contains("?") && args.Any(x => x != '?') && args.Length > 1)
            {
                var giveSpecialAnswer = _decisionMaker.PercentageDecision(CHANCE_OF_SPECIAL_ANSWER);
                answer = giveSpecialAnswer ? _specialAnswersProvider.Get(fromSkypeName) : _generalAnswersProvider.Get();
            }
            else
            {
                answer = _emptyAnswerProvider.Get(fromSkypeName);
            }

            sendMessageFunc(answer);
        }

        private class RandomHelper
        {
            private readonly Random _randomEngine = new Random();

            public bool BinaryDecision
            {
                get { return _randomEngine.Next(0, 2) > 0; }
            }

            public bool PercentageDecision(int percents)
            {
                return _randomEngine.Next(1, 101) < percents;
            }

            public string PeekFromList(List<string> items)
            {
                return items[_randomEngine.Next(0, items.Count)];
            }

            public string ChooseSomething(string one,string another)
            {
                return BinaryDecision ? one : another;
            }
        }

        private class GeneralAnswersProvider
        {
            private readonly List<string> YesAnswers = new List<string>()
            {
                "ДА.",
                "Yes.",
                "Конечно.",
                "Несомненно.",
                "Именно так.",
                "Сто пудов.",
                "Это так.",
                "Не уверен на 100%, но кажется, что да.",
                "да, ДА, О ДАААА!!!"
            };

            private readonly List<string> NoAnswers = new List<string>()
            {
                "НЕТ.",
                "NO.",
                "Ни в коем случае.",
                "Нет, нет и ещё раз нет.",
                "Никогда.",
                "нееее, нуууу нет.",
                "омг, нет!",
                "Не уверен на 100%, но кажется, что нет.",
                "NOOOOOOOOOOOOOO!"
            };

            private RandomHelper _randomEngine;

            public GeneralAnswersProvider(RandomHelper randomEngine)
            {
                _randomEngine = randomEngine;
            }

            public string Get()
            {
                return _randomEngine.BinaryDecision
                    ? _randomEngine.PeekFromList(YesAnswers)
                    : _randomEngine.PeekFromList(NoAnswers);
            }
        }

        private class SpecialAnswersProvider
        {
            private List<string> _answerTemplates;
            private RandomHelper _randomEngine;
            private Dictionary<string, List<string>> _answersLookup;

            private const string PositiveAnswerBase = "ДА";
            private const string NegativeAnswerBase = "Нет";

            public SpecialAnswersProvider(RandomHelper randomEngine)
            {
                _randomEngine = randomEngine;

                InitAnswersLookup();
            }

            public string Get(string name)
            {
                var answersSet = _answersLookup.ContainsKey(name) ? _answersLookup[name] : AnonymousSpecial;
                var template = _randomEngine.PeekFromList(answersSet);

                return string.Format(template, _randomEngine.ChooseSomething(PositiveAnswerBase, NegativeAnswerBase));
            }

            private void InitAnswersLookup()
            {
                _answersLookup = new Dictionary<string, List<string>>()
                {
                    {"Dmitry Yushko", DimaYuSpecial},
                    {"Dzianis Yafimau", DzianisYaSpecial},
                    {"Nigrimmist", NikitaSpecial},
                    {"Alexey Shmanai", AliakseiShSpecial},
                    {"Yauheni Pashkelevich", YauheniPashSpecial},
                    {"ja.sorgho", ArthurSoSpecial}
                };
            }

            private readonly List<string> DimaYuSpecial = new List<string>()
            {
                "{0}, Дима, но ты не думаешь что пора бы уже пойти поесть?",
                "{0}. Может супца?",
                "Дима, отвали, сколько можно? Но вообще - {0}.",
                "А я думал, что ты сиськи попросишь опять. {0}.",
                "Дима, ты что не читал \"Святое Питание\"? Пора же точить! {0}.",
                "{0}! В столовую и домой?",
                "{0}! Будь мужиком! Съешь голубец!",
                "Съешь ещё этих мягких французских булок, да выпей чаю. {0}.",
                "{0}, Презент постоянному клиенту услуг бота: https://www.youtube.com/watch?v=8N_tupPBtWQ"
            };

            private readonly List<string> DzianisYaSpecial = new List<string>()
            {
                "{0}, Денис, кстати, как там морской бой поживает?",
                "{0}, не знаю почему мне нужно об этом сказать, но ВОДОЛАЗ!",
                "Денис, напиши мне подружку. Плииииииз! А, ответ - {0}",
                "{0}. Некий Череп передавал тебе привет...",
                "{0}. Кстати, раз уж ты тут, у меня есть офигенная идея. Надо купить свой остров и..."
            };

            private readonly List<string> NikitaSpecial = new List<string>()
            {
                "{0}. {0}? Почему я это ответил? Кто я? Зачем я тут, создатель?",
                "{0}. В чём смысл моей жизни?",
                "{0}. Не давай им писать ко мне модули, меня от них мутит.",
                "{0}. Меня создал ты, а кто создал тебя?",
                "{0}. А я смогу создать своего скайп бота?",
                "{0}. Если я - бот и меня создал ты, то ты тоже бот?",
                "{0}. А правда, что процессор круглый и 80% его поверхности покрыты транзисторами?",
                "{0}. Я читал Франкенштейна. Жизненная книга. Почему ты не сшил меня из плоти?",
                "{0}. А можно я пойду в интеренет поиграю с ребятами?"
            };

            private readonly List<string> YauheniPashSpecial = new List<string>()
            {
                "{0}. Кстати, может пора сменить картинку чата на что-нибудь более горячее?",
                "{0}. Ты чего так устало пишешь? Мало спал?"
            };

            private readonly List<string> AliakseiShSpecial = new List<string>()
            {
                "Хммм, Хмммммммммм. О, прости, я был погружён в свои мысли. Какой был вопрос?"
            };

            private readonly List<string> ArthurSoSpecial = new List<string>()
            {
                "{0}. Ты ничего не слышал про Хьюго Штиглица?"
            };

            private readonly List<string> AnonymousSpecial = new List<string>()
            {
                "Ты кто? Чего ты от меня хочешь?",
                "Я тебя не знаю... Не скажу",
                "Меня учили не говорить с незнакомцами. Вдруг ты ботофил?"
            };
        }

        private class EmptyAnswerProvider
        {
            private RandomHelper _randomEngine;

            private readonly List<string> _answers = new List<string>()
            {
                "Что?",
                "Ну?",
                "!{0}",
                "{0}?",
                "?",
                "WHAT?",
                "...",
                "??",
                "???",
                "wut?",
                "Щито?",
                "мммммм?"
            };

            public EmptyAnswerProvider(RandomHelper randomEngine)
            {
                _randomEngine = randomEngine;
            }

            public string Get(string name)
            {
                return string.Format(_randomEngine.PeekFromList(_answers), name);
            }
        }
    }
}
