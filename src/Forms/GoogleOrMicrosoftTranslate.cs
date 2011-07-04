﻿using System;
using System.Windows.Forms;
using Nikse.SubtitleEdit.Logic;
using System.Drawing;

namespace Nikse.SubtitleEdit.Forms
{
    public sealed partial class GoogleOrMicrosoftTranslate : Form
    {
        public string TranslatedText { get; set; }

        public GoogleOrMicrosoftTranslate()
        {
            InitializeComponent();
            Forms.GoogleTranslate.FillComboWithGoogleLanguages(comboBoxFrom);
            Forms.GoogleTranslate.FillComboWithGoogleLanguages(comboBoxTo);
            RemovedLanguagesNotInMicrosoftTranslate(comboBoxFrom);
            RemovedLanguagesNotInMicrosoftTranslate(comboBoxTo);

            Text = Configuration.Settings.Language.GoogleOrMicrosoftTranslate.Title;
            labelGoogleTranslate.Text = Configuration.Settings.Language.GoogleOrMicrosoftTranslate.GoogleTranslate;
            labelMicrosoftTranslate.Text = Configuration.Settings.Language.GoogleOrMicrosoftTranslate.MicrosoftTranslate;
            labelFrom.Text = Configuration.Settings.Language.GoogleOrMicrosoftTranslate.From;
            labelTo.Text = Configuration.Settings.Language.GoogleOrMicrosoftTranslate.To;
            labelSourceText.Text = Configuration.Settings.Language.GoogleOrMicrosoftTranslate.SourceText;
            buttonGoogle.Text = Configuration.Settings.Language.GoogleOrMicrosoftTranslate.GoogleTranslate;
            buttonMicrosoft.Text = Configuration.Settings.Language.GoogleOrMicrosoftTranslate.MicrosoftTranslate;
            buttonTranslate.Text = Configuration.Settings.Language.GoogleOrMicrosoftTranslate.Translate;
            buttonCancel.Text = Configuration.Settings.Language.General.Cancel;
            FixLargeFonts();
        }

        private void FixLargeFonts()
        {
            Graphics graphics = this.CreateGraphics();
            SizeF textSize = graphics.MeasureString(buttonCancel.Text, this.Font);
            if (textSize.Height > buttonCancel.Height - 4)
            {
                int newButtonHeight = (int)(textSize.Height + 7 + 0.5);
                Utilities.SetButtonHeight(this, newButtonHeight, 1);
            }
        }

        private void RemovedLanguagesNotInMicrosoftTranslate(ComboBox comboBox)
        {
            for (int i = comboBox.Items.Count-1; i>0; i--)
            {
                Nikse.SubtitleEdit.Forms.GoogleTranslate.ComboBoxItem item = (Nikse.SubtitleEdit.Forms.GoogleTranslate.ComboBoxItem)comboBox.Items[i];
                if (item.Value != FixMsLocale(item.Value))
                    comboBox.Items.RemoveAt(i);
            }            
        }

        internal void InitializeFromLanguage(string defaultFromLanguage, string defaultToLanguage)
        {
            if (defaultFromLanguage == defaultToLanguage)
                defaultToLanguage = "en";

            int i = 0;
            comboBoxFrom.SelectedIndex = 0;
            foreach (Nikse.SubtitleEdit.Forms.GoogleTranslate.ComboBoxItem item in comboBoxFrom.Items)
            {
                if (item.Value == defaultFromLanguage)
                {
                    comboBoxFrom.SelectedIndex = i;
                    break;
                }
                i++;
            }

            i = 0;
            comboBoxTo.SelectedIndex = 0;
            foreach (Nikse.SubtitleEdit.Forms.GoogleTranslate.ComboBoxItem item in comboBoxTo.Items)
            {
                if (item.Value == defaultToLanguage)
                {
                    comboBoxTo.SelectedIndex = i;
                    break;
                }
                i++;
            }
        }

        internal void Initialize(Paragraph paragraph)
        {
            textBoxSourceText.Text = paragraph.Text;
        }

        private void GoogleOrMicrosoftTranslate_Shown(object sender, EventArgs e)
        {            
            Refresh();
            Translate();
        }

        private void Translate()
        {
            Cursor = Cursors.WaitCursor;
            try
            {
                string from = (comboBoxFrom.SelectedItem as Nikse.SubtitleEdit.Forms.GoogleTranslate.ComboBoxItem).Value;
                string to = (comboBoxTo.SelectedItem as Nikse.SubtitleEdit.Forms.GoogleTranslate.ComboBoxItem).Value;
                string languagePair = from + "|" + to;

                buttonGoogle.Text = string.Empty;
                buttonGoogle.Text = Forms.GoogleTranslate.TranslateTextViaApi(textBoxSourceText.Text, languagePair);

                GoogleTranslate gt = new GoogleTranslate();
                Subtitle subtitle = new Subtitle();
                subtitle.Paragraphs.Add(new Paragraph(0,0,textBoxSourceText.Text));
                gt.Initialize(subtitle, string.Empty, false);
                from = FixMsLocale(from);
                to = FixMsLocale(to);
                gt.DoMicrosoftTranslate(from, to);
                buttonMicrosoft.Text = gt.TranslatedSubtitle.Paragraphs[0].Text;
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private string FixMsLocale(string from)
        {
            if ("ar bg zh-CHS zh-CHT cs da nl en et fi fr de el ht he hu id it ja ko lv lt no pl pt ro ru sk sl es sv th tr uk vi".Contains(from))
                return from;
            return "en";
        }

        private void buttonTranslate_Click(object sender, EventArgs e)
        {
            Translate();
        }

        private void buttonGoogle_Click(object sender, EventArgs e)
        {
            TranslatedText = buttonGoogle.Text;
            DialogResult = DialogResult.OK;
        }

        private void buttonMicrosoft_Click(object sender, EventArgs e)
        {
            TranslatedText = buttonMicrosoft.Text;
            DialogResult = DialogResult.OK;
        }

        private void GoogleOrMicrosoftTranslate_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                DialogResult = DialogResult.Cancel;
        }

    }
}
