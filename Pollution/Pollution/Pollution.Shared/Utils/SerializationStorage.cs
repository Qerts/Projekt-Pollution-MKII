using System;
using System.Net;
using System.Windows;
using System.Windows.Input;
using System.IO;
using System.Runtime.Serialization;
using Windows.Storage;
using Windows.Storage.Search;
using Windows.Storage.Streams;
using System.Threading.Tasks;

namespace Utils
{
    public class SerializationStorage
    {
    public static void Serialize(Stream streamObject, object objForSerialization)
        {
            if (objForSerialization == null || streamObject == null)
                return;

            DataContractSerializer ser = new DataContractSerializer(objForSerialization.GetType());
            ser.WriteObject(streamObject, objForSerialization);
        }

        public static object Deserialize(Stream streamObject, Type serializedObjectType)
        {
            if (serializedObjectType == null || streamObject == null)
                return null;

            DataContractSerializer ser = new DataContractSerializer(serializedObjectType);
            return ser.ReadObject(streamObject);
        }

        public async static Task<object> Load(string filename, Type objectType)
        {
             try
	         {
                 var folder = Windows.Storage.ApplicationData.Current.LocalFolder;
                 var file = await folder.GetFileAsync(filename);

                 using (var stream = await file.OpenReadAsync())
                 {

                     object o = Deserialize(stream.AsStream(), objectType);

                     stream.Dispose();

                     return o;
                 }

	        }
	        catch (Exception e)
	        {
                return null;
	        }
        }

        public async static Task<bool> Save(string filename, object o)
        {
            try
            {
                var folder = Windows.Storage.ApplicationData.Current.LocalFolder;
                var file = await folder.CreateFileAsync(filename, CreationCollisionOption.ReplaceExisting);
                using (var stream = await file.OpenStreamForWriteAsync())
                {
                    if (o == null || stream == null)
                    {
                    }
                    else
                    {
                        DataContractSerializer ser = new DataContractSerializer(o.GetType());
                        ser.WriteObject(stream, o);
                    }
                    stream.Dispose();
                    return true;
                }
            }
            catch (Exception e)
            {
                return false;
            }
        }

    }
}
