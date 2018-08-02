using JsonFx.Json;
using JsonFx.Serialization.Resolvers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace TABHax.Dumping
{
    public class SceneDumper
    {

        public static void Dump()
        {
            Console.WriteLine("Dumping...");
            var gameObjects = GameObject.FindObjectsOfType<GameObject>();
            Console.WriteLine("{0} GameObjects in scene...", gameObjects.Length);
            var dumped = new List<DGameObject>();
            foreach (var g in gameObjects)
            {
                if (g.transform.parent == null)
                {
                    try
                    {

                        dumped.Add(DGameObject.Copy(g));
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        Console.WriteLine(ex.StackTrace);
                    }
                }
            }
            Console.WriteLine("Done, dumped {0} GameObjects", dumped.Count);

            try
            {
                var fileName = string.Format("{0}.json", DateTime.Now.Ticks);
                var writer = new JsonWriter(new JsonFx.Serialization.DataWriterSettings(new ConventionResolverStrategy(ConventionResolverStrategy.WordCasing.NoChange)));
                var str = writer.Write(dumped.ToArray());
                File.WriteAllText(fileName, str);
                Console.WriteLine("Wrote to {0}", fileName);
            }catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
            }
        }
    }
}
