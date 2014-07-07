using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.DirectX.DirectSound;
using System.IO;

namespace AudioTest
{
    class Program
    {


        public static void  Main(String[] args)
        {
            /*
            DevicesCollection devs = new DevicesCollection();
            foreach (DeviceInformation d in devs)
            {
                Console.WriteLine(d);
            }
            */

            Capture soundCard = new Capture(new DevicesCollection()[0].DriverGuid);
            //soundCard.SetCooperativeLevel(this, CooperativeLevel.Normal);

            WaveFormat wf = new WaveFormat();
            wf.FormatTag = WaveFormatTag.Pcm;
            wf.SamplesPerSecond = 44100; //44.1 Hz (3 standard Hz formats)
            wf.BitsPerSample = 16; // 8 or 16 bit resolution
            wf.Channels = 1; //mono (2 = stereo)
            wf.BlockAlign = (short)((wf.Channels * wf.BitsPerSample) / 8); //pcm standard
            wf.AverageBytesPerSecond = wf.BlockAlign * wf.SamplesPerSecond; //pcm standard

            CaptureBufferDescription bufferDesc = new CaptureBufferDescription();
            bufferDesc.Format = wf;
            bufferDesc.BufferBytes = (wf.BitsPerSample / 8) * wf.SamplesPerSecond * 5; //<--seconds
            bufferDesc.WaveMapped = false;
            bufferDesc.ControlEffects = false;

            CaptureBuffer buffer = new CaptureBuffer(bufferDesc, soundCard);
            buffer.Start(false);
            while (buffer.Capturing)
            {
                System.Threading.Thread.Sleep(100);
            }
            buffer.Stop();

            String filePath = @"C:\Users\ACox\Desktop\AudioLuv Repository\Test.wav";
            if(File.Exists(filePath)){
                File.Delete(filePath);
            }

            using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
            {
                //write wav headers here
                buffer.Read(0, fileStream, buffer.Caps.BufferBytes, LockFlag.None);
                buffer.Dispose();
            }

            Console.WriteLine("Done");
            Console.ReadKey();
        }
    }
}
