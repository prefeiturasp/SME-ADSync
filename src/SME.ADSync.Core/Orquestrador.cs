﻿using SME.ADSync.Core.Enumerados;
using SME.ADSync.Core.Interfaces;
using SME.ADSync.Core.Processors;
using System;
using System.Collections.Concurrent;

namespace SME.ADSync.Core
{
    public static class Orquestrador
    {
        private static ConcurrentDictionary<TipoProcessamento, IProcessor> processadores;

        static Orquestrador()
        {
            processadores = new ConcurrentDictionary<TipoProcessamento, IProcessor>();
        }

        public static IServiceProvider Provider { get; private set; }
        public static void Desativar()
        {
            processadores.Clear();
            Registrar(new DisabledProcessor());
        }

        public static void Inicializar(IServiceProvider provider)
        {
            if (Provider == null)
                Provider = provider;
        }

        public static IProcessor ObterProcessador(TipoProcessamento tipoProcessamento)
        {
            IProcessor processador = null;

            if (processadores.TryGetValue(tipoProcessamento, out processador))
                return processador;
            else
                throw new Exception($"Não foi possível obter um processador do tipo {tipoProcessamento.ToString()} pois não foi registrado");
        }

        public static void Registrar<T>(T processador)
            where T : IProcessor
        {
            Registrar<T>(processador, TipoProcessamento.ExecucaoImediata);
            Registrar<T>(processador, TipoProcessamento.ExecucaoRecorrente);
        }

        public static void Registrar<T>(T processador, TipoProcessamento tipoProcessamento)
            where T : IProcessor
        {
            if (processadores.TryAdd(tipoProcessamento, processador) && !processador.Registrado)
            {
                processador.Registrar();
                Console.WriteLine($"O processador {processador.GetType().Name} foi registrado para o tipo de processamento {tipoProcessamento.ToString()}");
            }
        }
    }
}
