using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Nexus.Extensions
{
    public class RadioButtonGroup
    {
        public string Name { get; private set; }
        public RadioButton? Selected { get; private set; }
        public List<RadioButton> RadioButtons { get; private set; } = [];
        public event Action<RadioButton?>? SelectionChanged;
        public RadioButtonGroup(string name) { Name = name; }
        public RadioButtonGroup(string name, IEnumerable<RadioButton> radioButtons) {
            Name = name;
            foreach (RadioButton button in radioButtons) RadioButtons.Add(button);
        }

		public void Add(RadioButton radioButton)
        {
            if (radioButton == null || radioButton.GroupName != Name) 
                throw new ArgumentException(null, nameof(radioButton));
            RadioButtons.Add(radioButton);
            radioButton.Checked += (object sender, RoutedEventArgs e) =>
            {
                Selected = radioButton;
                SelectionChanged?.Invoke(radioButton);
            };
        }
        public void Add(IEnumerable<RadioButton> radioButtons)
        {
            foreach (RadioButton radioButton in radioButtons) Add(radioButton);
        }
    }
}
