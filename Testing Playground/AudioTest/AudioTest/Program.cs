using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.DirectX.DirectSound;

namespace AudioTest
{
    class Program
    {


        public static void  Main(String[] args)
        {
            Capture capture = new Capture();
            CaptureBufferDescription desc = new CaptureBufferDescription();
            CaptureBuffer buffer = new CaptureBuffer(desc, capture);
            Console.WriteLine(buffer.Caps.BufferBytes);
            Console.ReadKey();
        }
    }
}
