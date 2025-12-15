using CreditoConstituido.Domain.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreditoConstituido.Domain.Entities;

public sealed class Credito
{
    public long Id { get; private set; }
    public string NumeroCredito { get; private set; }
    public string NumeroNfse { get; private set; }
    public DateOnly DataConstituicao { get; private set; }
    public decimal ValorIssqn { get; private set; }
    public string TipoCredito { get; private set; }
    public bool SimplesNacional { get; private set; }
    public decimal Aliquota { get; private set; }
    public decimal ValorFaturado { get; private set; }
    public decimal ValorDeducao { get; private set; }
    public decimal BaseCalculo { get; private set; }

    private Credito()
    {
        NumeroCredito = string.Empty;
        NumeroNfse = string.Empty;
        TipoCredito = string.Empty;
    }

    public Credito(
        string numeroCredito,
        string numeroNfse,
        DateOnly dataConstituicao,
        decimal valorIssqn,
        string tipoCredito,
        bool simplesNacional,
        decimal aliquota,
        decimal valorFaturado,
        decimal valorDeducao,
        decimal baseCalculo)
    {
        if (string.IsNullOrWhiteSpace(numeroCredito)) throw new DomainException("numeroCredito é obrigatório");
        if (string.IsNullOrWhiteSpace(numeroNfse)) throw new DomainException("numeroNfse é obrigatório");
        if (string.IsNullOrWhiteSpace(tipoCredito)) throw new DomainException("tipoCredito é obrigatório");

        NumeroCredito = numeroCredito.Trim();
        NumeroNfse = numeroNfse.Trim();
        DataConstituicao = dataConstituicao;
        ValorIssqn = valorIssqn;
        TipoCredito = tipoCredito.Trim();
        SimplesNacional = simplesNacional;
        Aliquota = aliquota;
        ValorFaturado = valorFaturado;
        ValorDeducao = valorDeducao;
        BaseCalculo = baseCalculo;
    }
}