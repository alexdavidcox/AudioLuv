using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.DirectX.DirectSound;
using System.IO;
using System.Collections;

namespace AudioTest
{
    class Program
    {

        //Machine is LittleEndian
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
            bufferDesc.BufferBytes = (wf.BitsPerSample / 8) * wf.SamplesPerSecond * 1; //<--seconds
            bufferDesc.WaveMapped = false;
            bufferDesc.ControlEffects = false;

            CaptureBuffer buffer = new CaptureBuffer(bufferDesc, soundCard);
            buffer.Start(false);
            while (buffer.Capturing)
            {
                System.Threading.Thread.Sleep(100);
            }
            buffer.Stop();
            
            List<Byte> byteArray = new List<Byte>();

            //RIFF Header
            byteArray.AddRange(Encoding.UTF8.GetBytes("RIFF")); //bigE
            byteArray.AddRange(BitConverter.GetBytes(0)); //overall size
            byteArray.AddRange(Encoding.UTF8.GetBytes("WAVE")); //bigE

            //fmt chunk
            byteArray.AddRange(Encoding.UTF8.GetBytes("fmt")); //bigE
            byteArray.AddRange(BitConverter.GetBytes(16)); //PCM length always 16 byte
            byteArray.AddRange(BitConverter.GetBytes(1)); //means no compression
            byteArray.AddRange(BitConverter.GetBytes(wf.Channels));
            byteArray.AddRange(BitConverter.GetBytes(wf.SamplesPerSecond));
            byteArray.AddRange(BitConverter.GetBytes(wf.AverageBytesPerSecond));
            byteArray.AddRange(BitConverter.GetBytes(wf.BlockAlign));
            byteArray.AddRange(BitConverter.GetBytes(wf.BitsPerSample));

            //data chunk
            byteArray.AddRange(Encoding.UTF8.GetBytes("data")); //bigE
            byteArray.AddRange(BitConverter.GetBytes(buffer.Caps.BufferBytes));

            Byte[] header = byteArray.ToArray();

            String filePath = @"C:\Users\ACox\Desktop\AudioLuv Repository\Test.wav";
            if(File.Exists(filePath)){
                File.Delete(filePath);
            }

            //Byte[] data = buffer.Read(0, System.Type.GetType("Byte[]"), buffer.Caps.BufferBytes, LockFlag.None);
            using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
            {
                fileStream.Write(header,0,header.Length);
                fileStream.Flush();
                buffer.Read(0, fileStream, buffer.Caps.BufferBytes, LockFlag.None);
                fileStream.Flush();
                buffer.Dispose();
            }
            //Console.WriteLine("Done");
            //Console.ReadKey();
        }
    }
}
