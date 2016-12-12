using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Speech.Recognition;
using System.Speech.Synthesis;
using System.IO;

namespace SpeechRecog
{
    public partial class Form1 : Form
    {
        SpeechRecognitionEngine _recognizer = new SpeechRecognitionEngine();
        SpeechSynthesizer VAS = new SpeechSynthesizer();
        /// <summary>
        /// Above is init of speech dude.
        /// </summary>
        
        public Form1()
        {
            //"Show me" activate pointing
            Choices pointActivate = new Choices("Show me");
            
            //Zooming
            Choices Zooming = new Choices("Hello", "Zoom-in", "Zoom-out");
            
            //Route Search
            Choices cities = new Choices(new string[] {"Berlin","Barcelona","Paris","London","Beijing"});
            GrammarBuilder GB_zooming = new GrammarBuilder(Zooming);
            GrammarBuilder GB_route = new GrammarBuilder();
            GrammarBuilder GB_point = new GrammarBuilder(); 

            GB_route.Append("from");
            GB_route.Append(cities);
            GB_route.Append("to");
            GB_route.Append(cities);

            GB_point.Append(pointActivate);
            GB_point.Append(cities);
           
           
            Grammar SudeepGrammer = new Grammar(GB_zooming);
            Grammar routeGrammar = new Grammar(GB_route);
            routeGrammar.Name = ("Route Search");
            Grammar pointGrammar = new Grammar(GB_point);
            pointGrammar.Name = ("Show a place");

            _recognizer.SetInputToDefaultAudioDevice();
            _recognizer.LoadGrammar(SudeepGrammer);
            _recognizer.LoadGrammar(routeGrammar);
            _recognizer.LoadGrammar(pointGrammar);
            _recognizer.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(_recognizer_SpeechRecognized);
            _recognizer.RecognizeAsync(RecognizeMode.Multiple);
            InitializeComponent();
        }
        void _recognizer_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            string speech = e.Result.Text;
            MessageBox.Show(speech);
        }
        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
