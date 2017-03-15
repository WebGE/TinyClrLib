using System;
using System.Collections;
using System.Diagnostics;
using GHIElectronics.TinyCLR.Devices.Enumeration;
using GHIElectronics.TinyCLR.Devices.I2c;

namespace Application
{
    public class I2cScanner
    {
        public static ArrayList Ping(string filter)
        {
            ArrayList list = new ArrayList();
            var devices = DeviceInformation.FindAll(filter);
            byte[] buffer = new byte[1];
            for (int i = 0x03; i < 0x77; i++)
            {
                I2cConnectionSettings settings = new I2cConnectionSettings(i);
                I2cDevice device = I2cDevice.FromId(devices[0].Id, settings);
                try
                {
                    var res = device.WritePartial(new byte[]{0x00});
                    if (res.BytesTransferred>0)
                        list.Add(i);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
                finally
                {
                    device.Dispose();
                    device = null;
                }
            }
            return list;
        }
    }

}