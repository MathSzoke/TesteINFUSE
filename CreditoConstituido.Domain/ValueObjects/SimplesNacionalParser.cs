using CreditoConstituido.Domain.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreditoConstituido.Domain.ValueObjects;

public static class SimplesNacionalParser
{
    public static bool Parse(string value)
    {
        if (string.IsNullOrWhiteSpace(value)) throw new DomainException("simplesNacional é obrigatório");
        var v = value.Trim();

        if (string.Equals(v, "Sim", StringComparison.OrdinalIgnoreCase)) return true;
        if (string.Equals(v, "Não", StringComparison.OrdinalIgnoreCase)) return false;
        if (string.Equals(v, "Nao", StringComparison.OrdinalIgnoreCase)) return false;

        throw new DomainException("simplesNacional deve ser 'Sim' ou 'Não'");
    }
}