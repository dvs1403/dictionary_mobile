using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;
using Xamarin.Forms;
using MobileApp.Models;

namespace MobileApp
{
    [DesignTimeVisible(false)]
    public partial class MainPage : Xamarin.Forms.TabbedPage
    {
        public List<Word> WordModels { get; set; }
        Random rnd = new Random();
        public int currentNumber = 0;
        public bool isShowAnswer;
        public MainPage()
        {
            InitializeComponent();
            On<Android>().SetToolbarPlacement(ToolbarPlacement.Bottom);
            On<Android>().SetBarSelectedItemColor(Color.FromHex("#6A5ACD"));

            WordModels = new List<Word>();
            wordsList.ItemsSource = GetListWords().ToArray();
        }

        private async void AddWord_Click(object sender,  EventArgs e)
        {
            string result = await DisplayPromptAsync("Add new word", "Write a word!");
            if (result == "" || result == null)
            {
                wordsList.ItemsSource = GetListWords().ToArray();
                wordsList.SelectedItem = null;
                return;
            }
            if (SearchWord(result))
            {
                DisplayAlert("Notification", "This word is already on the dictionary!", "ОK");
                return;
            }

            string result2 = await DisplayPromptAsync("Add new word", "Write the translation of the word!");
            WordModels.Add(new Word(result, result2));
            wordsList.ItemsSource = GetListWords().ToArray();
            CountWordsLabel.Text = $"Word count: {GetCountWords()}";
            currentNumber = rnd.Next(0, GetCountWords());
            FlashcardLabel.Text = WordModels[currentNumber].FirstWord;
        }

        private async void WordsList_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem != null)
            {
                string word = e.SelectedItem.ToString();
                bool result = await DisplayAlert("Подтвердить действие", $"Вы хотите удалить элемент {word}?", "Да", "Нет");
                if (!result)
                {
                    wordsList.SelectedItem = null;
                    return;
                }
                var list = GetListWords();
                for(int i = 0;i < list.Count; i++)
                {
                    if(word == list[i])
                    {
                        WordModels.RemoveAt(i);
                        wordsList.ItemsSource = GetListWords().ToArray();
                        CountWordsLabel.Text = $"Word count: {GetCountWords()}";
                        CountLearnWordsLabel.Text = $"Learned word count: {GetCountLearnWords()}";
                        if (GetCountWords() == 0)
                        {
                            FlashcardLabel.Text = "";

                            return;
                        }
                        currentNumber = rnd.Next(0, GetCountWords());
                        FlashcardLabel.Text = WordModels[currentNumber].FirstWord;
                    }
                }
            }
        }

        private async void CheckFlashcard(object sender, EventArgs e)
        {
            if (GetCountWords() == 0)
                return;
            if (!isShowAnswer)
            {
                isShowAnswer = true;
                FlashcardLabel.Text = WordModels[currentNumber].SecondWord;
            }
            else
            {
                isShowAnswer = false;
                WordModels[currentNumber].IsLearn = true;
                currentNumber = rnd.Next(0, GetCountWords());
                FlashcardLabel.Text = WordModels[currentNumber].FirstWord;
            }
            CountLearnWordsLabel.Text = $"Learned word count: {GetCountLearnWords()}";
        }

        private async void EditProfile(object sender, EventArgs e)
        {
            string result = await DisplayPromptAsync("Edit profile", "Write your name!");
            if (result == "" || result == null)
            {
                return;
            }
            Name.Text = result;
        }

        private List<string> GetListWords()
        {
            var list = new List<String>();
            foreach(var word in WordModels)
            {
                list.Add($"{word.FirstWord} - {word.SecondWord}");
            }

            return list;
        }
        private bool SearchWord(string wordString)
        {
            bool result = false;
            foreach(var word in WordModels)
            {
                if (word.FirstWord == wordString)
                    result = true;
            }
            return result;
        }
        private int GetCountWords()
        {
            return WordModels.Count;
        }

        private int GetCountLearnWords()
        {
            int count = 0;

            foreach(var word in WordModels)
            {
                if (word.IsLearn)
                    count++;
            }

            return count;
        }
        
    }
}
