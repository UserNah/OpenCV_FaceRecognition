using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
namespace OpenCVWinForm
{

    public partial class Form1 : Form
    {

        Image<Bgr, Byte> currentFrame;
        VideoCapture camera;

        CascadeClassifier face;
        CascadeClassifier eye;
        CascadeClassifier smile;
        public Form1()
        {
            face = new CascadeClassifier("haarcascade_frontalface_default.xml");
            eye = new CascadeClassifier("haarcascade_eye.xml");
            smile = new CascadeClassifier("haarcascade_smile.xml");

            
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            camera = new VideoCapture();
            Application.Idle += Application_Idle;
            button1.Enabled = false;
        }
        private void Application_Idle(object sender, EventArgs e)
        {
            currentFrame = camera.QueryFrame().ToImage<Emgu.CV.Structure.Bgr, byte>().Resize(640, 480, Emgu.CV.CvEnum.Inter.Cubic);

            Rectangle[] facesDetected = face.DetectMultiScale(currentFrame, 1.2, 10, new Size(10, 10), new Size(640, 480));

            foreach (Rectangle f in facesDetected)
            {
                currentFrame.Draw(f, new Bgr(Color.Red), 2);
                Rectangle[] eyesDetected = eye.DetectMultiScale(currentFrame, 1.2, 10, new Size(8, 8), new Size(160, 120));

                if (eyesDetected.Length <= 1)
                {
                    currentFrame.Draw("Eyes closed", new Point(f.X, f.Y), FontFace.HersheyTriplex, 1.0, new Bgr(Color.Cyan));
                }

                foreach (Rectangle eyeRect in eyesDetected)
                {
                    eyeRect.Inflate(-7, -7);
                    currentFrame.Draw(eyeRect, new Bgr(Color.Blue), 2);
                }

                currentFrame.ROI = f;
                Rectangle[] smileDetected = smile.DetectMultiScale(currentFrame, 1.33, 40, new Size(16, 12), new Size(320, 240));

                foreach (Rectangle smileRect in smileDetected)
                {
                    currentFrame.Draw(smileRect, new Bgr(Color.Black), 2);
                    currentFrame.Draw("SMILE", new Point(smileRect.X, smileRect.Y), FontFace.HersheyPlain, 1.5, new Bgr(Color.Yellow));
                }

                currentFrame.ROI = Rectangle.Empty;
            }

            panel1.Image = currentFrame;
        }
    }
}
