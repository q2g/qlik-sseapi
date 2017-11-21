namespace Qlik.Sse
{
    #region Usings
    using Google.Protobuf;
    using Grpc.Core;
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    #endregion

    #region ProtobufHelper
    public static class ProtobufHelper
    {
        private static ConcurrentDictionary<Type, Tuple<string, MessageParser>> parserDic = new ConcurrentDictionary<Type, Tuple<string, MessageParser>>();
        public static T ParseIMessageFirstOrDefault<T>(this Metadata md) where T : class, Google.Protobuf.IMessage<T>, new()
        {
            var type = typeof(T);
            Tuple<string, MessageParser> parser = null;
            if (parserDic.ContainsKey(type))
            {
                parser = parserDic[type];
            }
            else
            {
                parser = new Tuple<string, MessageParser>($"qlik-{type.Name.ToLowerInvariant()}-bin", new MessageParser<T>(() => new T()));
                parserDic.TryAdd(type, parser);
            }

            foreach (var item in md)
            {
                if (item.Key == parser?.Item1)
                {
                    return parser?.Item2?.ParseFrom(item.ValueBytes) as T;
                }
            }

            return default(T);
        }

        public static IEnumerable<T> ParseIMessages<T>(this Metadata md) where T : class, Google.Protobuf.IMessage<T>, new()
        {
            var type = typeof(T);
            Tuple<string, MessageParser> parser = null;
            if (parserDic.ContainsKey(type))
            {
                parser = parserDic[type];
            }
            else
            {
                parser = new Tuple<string, MessageParser>($"qlik-{type.Name.ToLowerInvariant()}-bin", new MessageParser<T>(() => new T()));
                parserDic.TryAdd(type, parser);
            }

            foreach (var item in md)
            {
                if (item.Key == parser?.Item1)
                {
                    yield return parser?.Item2?.ParseFrom(item.ValueBytes) as T;
                }
            }

            yield return default(T);
        }

    }
    #endregion
}
