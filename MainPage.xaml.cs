using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using Microsoft.Maui.Controls;
using System.Diagnostics;

namespace MauiApp1
{
    public partial class MainPage : ContentPage, INotifyPropertyChanged
    {
        private string _currentInput = string.Empty;
        private List<string> _history = new List<string>();
        private bool isEnteringExponent = false;

        public string CurrentInput
        {
            get { return _currentInput; }
            set
            {
                if (_currentInput != value)
                {
                    _currentInput = value;
                    OnPropertyChanged(nameof(CurrentInput));
                }
            }
        }

        public MainPage()
        {
            InitializeComponent();  
            BindingContext = this;
        }

        private void OnButtonClicked(object sender, EventArgs e)
        {
            if (sender is Button button)
            {
                string parameter = button.CommandParameter?.ToString();
                Debug.WriteLine("OnButtonClicked: początek");
                switch (parameter)
                {
                    case "=":
                        Evaluate();
                        break;

                    case "C":
                        ClearAll();
                        break;

                    case "^":
                        isEnteringExponent = true;
                        CurrentInput += "^";
                        break;

                    case "History":
                        DisplayAlert("History", string.Join(Environment.NewLine, _history), "OK");
                        break;

                    case "Save":
                        SaveHistoryToFile("history.txt");
                        break;

                    case "Load":
                        LoadHistoryFromFile("history.txt");
                        break;

                    default:
                        CurrentInput += parameter;
                        break;
                }
            }
        }

        private void Evaluate()
        {
            try
            {
                if (isEnteringExponent) 
                {
                    string[] operands = CurrentInput.Split('^');
                    
                    if (operands.Length == 2)
                    {
                        double baseNumber, exponent;
                        if (double.TryParse(operands[0], out baseNumber) && double.TryParse(operands[1], out exponent))
                        {
                            double result = Math.Pow(baseNumber, exponent);
                            string operation = $"{CurrentInput} = {result}";
                            _history.Add(operation);
                            CurrentInput = result.ToString();
                        }
                        else
                        {
                            CurrentInput = "Błąd";
                        }
                    }
                    else
                    {
                        CurrentInput = "Błąd";
                    }

                    isEnteringExponent = false;
                }
                else
                {
                    DataTable table = new DataTable();
                    var result = table.Compute(CurrentInput, "");
                    string operation = $"{CurrentInput} = {result}";
                    _history.Add(operation);
                    CurrentInput = result.ToString();
                }
            }
            catch (Exception ex)
            {
                CurrentInput = "Błąd";
            }
        }

        private void ClearAll()
        {
            CurrentInput = string.Empty;
            _history.Clear();
            isEnteringExponent = false;
        }

        private void SaveHistoryToFile(string fileName)
        {
            if (_history != null)
            {
                string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                string filePath = Path.Combine(folderPath, fileName);
                Debug.WriteLine("SAVE DZIALA");
                File.WriteAllLines(filePath, _history);
            }
            else
            {
                //COS ZROB JAK BEDZIE ZLE 
            }
        }

        private void LoadHistoryFromFile(string fileName)
        {
            try
            {
                string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                string filePath = Path.Combine(folderPath, fileName);

                if (File.Exists(filePath))
                {
                    var lines = File.ReadAllLines(filePath);
                    _history.Clear();
                    _history.AddRange(lines);
                    Console.WriteLine("Plik wczytany pomyślnie.");
                }
                else
                {
                    Console.WriteLine("Plik historii nie istnieje.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Błąd odczytu z pliku: {ex.Message}");
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
