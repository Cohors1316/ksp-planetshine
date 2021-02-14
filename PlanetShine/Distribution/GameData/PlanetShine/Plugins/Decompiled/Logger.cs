using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace PlanetShine
{
  [KSPAddon]
  public class Logger : MonoBehaviour
  {
    private static readonly string fileName;
    private static readonly AssemblyName assemblyName;
    private static readonly List<string[]> messages = new List<string[]>();

    static Logger()
    {
      Logger.assemblyName = Assembly.GetExecutingAssembly().GetName();
      Logger.fileName = Path.ChangeExtension(Assembly.GetExecutingAssembly().Location, "log");
      File.Delete(Logger.fileName);
      lock (Logger.messages)
      {
        Logger.messages.Add(new string[1]
        {
          "Executing: " + Logger.assemblyName.Name + " - " + (object) Logger.assemblyName.Version
        });
        Logger.messages.Add(new string[1]
        {
          "Assembly: " + Assembly.GetExecutingAssembly().Location
        });
      }
      Logger.Blank();
    }

    private void Awake() => Object.DontDestroyOnLoad((Object) this);

    public static void Blank()
    {
      lock (Logger.messages)
        Logger.messages.Add(new string[0]);
    }

    public static void Log(object obj)
    {
      lock (Logger.messages)
      {
        try
        {
          if (obj is IEnumerable)
          {
            Logger.messages.Add(new string[2]
            {
              "Log " + (object) DateTime.Now.TimeOfDay,
              obj.ToString()
            });
            foreach (object obj1 in obj as IEnumerable)
              Logger.messages.Add(new string[2]
              {
                "\t",
                obj1.ToString()
              });
          }
          else
            Logger.messages.Add(new string[2]
            {
              "Log " + (object) DateTime.Now.TimeOfDay,
              obj.ToString()
            });
        }
        catch (System.Exception ex)
        {
          Logger.Exception(ex);
        }
      }
    }

    public static void Log(string name, object obj)
    {
      lock (Logger.messages)
      {
        try
        {
          if (obj is IEnumerable)
          {
            Logger.messages.Add(new string[2]
            {
              "Log " + (object) DateTime.Now.TimeOfDay,
              name
            });
            foreach (object obj1 in obj as IEnumerable)
              Logger.messages.Add(new string[2]
              {
                "\t",
                obj1.ToString()
              });
          }
          else
            Logger.messages.Add(new string[2]
            {
              "Log " + (object) DateTime.Now.TimeOfDay,
              obj.ToString()
            });
        }
        catch (System.Exception ex)
        {
          Logger.Exception(ex);
        }
      }
    }

    public static void Log(string message)
    {
      lock (Logger.messages)
        Logger.messages.Add(new string[2]
        {
          "Log " + (object) DateTime.Now.TimeOfDay,
          message
        });
    }

    [Conditional("DEBUG")]
    [DebuggerStepThrough]
    public static void Debug(string message)
    {
      lock (Logger.messages)
        Logger.messages.Add(new string[2]
        {
          "Debug " + (object) DateTime.Now.TimeOfDay,
          message
        });
    }

    public static void Warning(string message)
    {
      lock (Logger.messages)
        Logger.messages.Add(new string[2]
        {
          "Warning " + (object) DateTime.Now.TimeOfDay,
          message
        });
    }

    public static void Error(string message)
    {
      lock (Logger.messages)
        Logger.messages.Add(new string[2]
        {
          "Error " + (object) DateTime.Now.TimeOfDay,
          message
        });
    }

    public static void Exception(System.Exception ex)
    {
      lock (Logger.messages)
      {
        Logger.messages.Add(new string[2]
        {
          "Exception " + (object) DateTime.Now.TimeOfDay,
          ex.Message
        });
        Logger.messages.Add(new string[2]
        {
          string.Empty,
          ex.StackTrace
        });
        Logger.Blank();
      }
    }

    public static void Exception(System.Exception ex, string location)
    {
      lock (Logger.messages)
      {
        Logger.messages.Add(new string[2]
        {
          "Exception " + (object) DateTime.Now.TimeOfDay,
          location + " // " + ex.Message
        });
        Logger.messages.Add(new string[2]
        {
          string.Empty,
          ex.StackTrace
        });
        Logger.Blank();
      }
    }

    public static void Flush()
    {
      lock (Logger.messages)
      {
        if (Logger.messages.Count <= 0)
          return;
        using (StreamWriter streamWriter = File.AppendText(Logger.fileName))
        {
          foreach (string[] message in Logger.messages)
          {
            streamWriter.WriteLine(message.Length != 0 ? (message.Length > 1 ? "[" + message[0] + "]: " + message[1] : message[0]) : string.Empty);
            if ((uint) message.Length > 0U)
              MonoBehaviour.print(message.Length > 1 ? (object) (Logger.assemblyName.Name + " -> " + message[1]) : (object) (Logger.assemblyName.Name + " -> " + message[0]));
          }
        }
        Logger.messages.Clear();
      }
    }

    private void LateUpdate() => Logger.Flush();

    private void OnDestroy() => Logger.Flush();

    ~Logger()
    {
      try
      {
        Logger.Flush();
      }
      finally
      {
        // ISSUE: explicit finalizer call
        // ISSUE: explicit non-virtual call
        __nonvirtual (((object) this).Finalize());
      }
    }

    public Logger() => base.\u002Ector();
  }
}
