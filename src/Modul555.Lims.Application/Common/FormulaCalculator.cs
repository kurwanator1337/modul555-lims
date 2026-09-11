using System.Globalization;
using System.Text.RegularExpressions;

namespace Modul555.Lims.Application.Common;

/// <summary>
/// Вычисляет формулу метода над измеряемыми величинами.
/// Поддерживает +, -, *, /, скобки и имена переменных (mass, volume, force).
/// </summary>
public static class FormulaCalculator
{
    private static readonly Regex Token = new(
        @"[A-Za-z_][A-Za-z0-9_]*|\d+(?:\.\d+)?|[+\-*/()]",
        RegexOptions.Compiled
    );

    public static decimal Evaluate(string? formula, IReadOnlyDictionary<string, decimal> variables)
    {
        if (string.IsNullOrWhiteSpace(formula))
        {
            if (variables.Count == 1)
                return variables.Values.First();
            throw new InvalidOperationException(
                "Формула не задана, а измеряемых величин больше одной."
            );
        }

        var tokens = Token.Matches(formula.Replace(',', '.')).Select(m => m.Value).ToList();
        if (tokens.Count == 0)
            throw new InvalidOperationException($"Не удалось разобрать формулу: {formula}");

        var rpn = ToRpn(tokens, variables);
        return EvalRpn(rpn);
    }

    private static List<string> ToRpn(
        List<string> tokens,
        IReadOnlyDictionary<string, decimal> variables
    )
    {
        var output = new List<string>();
        var ops = new Stack<string>();
        var precedence = new Dictionary<string, int>
        {
            ["+"] = 1,
            ["-"] = 1,
            ["*"] = 2,
            ["/"] = 2,
        };

        foreach (var raw in tokens)
        {
            var token = raw;
            if (decimal.TryParse(token, NumberStyles.Any, CultureInfo.InvariantCulture, out _))
            {
                output.Add(token);
                continue;
            }

            if (char.IsLetter(token[0]) || token[0] == '_')
            {
                if (!variables.TryGetValue(token, out var value))
                    throw new InvalidOperationException(
                        $"В формуле используется неизвестная переменная «{token}»."
                    );
                output.Add(value.ToString(CultureInfo.InvariantCulture));
                continue;
            }

            if (token == "(")
            {
                ops.Push(token);
                continue;
            }

            if (token == ")")
            {
                while (ops.Count > 0 && ops.Peek() != "(")
                    output.Add(ops.Pop());
                if (ops.Count == 0)
                    throw new InvalidOperationException("Непарные скобки в формуле.");
                ops.Pop();
                continue;
            }

            while (
                ops.Count > 0
                && ops.Peek() != "("
                && precedence.GetValueOrDefault(ops.Peek()) >= precedence[token]
            )
                output.Add(ops.Pop());
            ops.Push(token);
        }

        while (ops.Count > 0)
        {
            var op = ops.Pop();
            if (op is "(" or ")")
                throw new InvalidOperationException("Непарные скобки в формуле.");
            output.Add(op);
        }

        return output;
    }

    private static decimal EvalRpn(List<string> rpn)
    {
        var stack = new Stack<decimal>();
        foreach (var token in rpn)
        {
            if (
                decimal.TryParse(
                    token,
                    NumberStyles.Any,
                    CultureInfo.InvariantCulture,
                    out var number
                )
            )
            {
                stack.Push(number);
                continue;
            }

            if (stack.Count < 2)
                throw new InvalidOperationException("Некорректная формула.");
            var b = stack.Pop();
            var a = stack.Pop();
            stack.Push(
                token switch
                {
                    "+" => a + b,
                    "-" => a - b,
                    "*" => a * b,
                    "/" => b == 0
                        ? throw new InvalidOperationException("Деление на ноль в формуле.")
                        : a / b,
                    _ => throw new InvalidOperationException($"Неизвестный оператор {token}."),
                }
            );
        }

        if (stack.Count != 1)
            throw new InvalidOperationException("Некорректная формула.");
        return stack.Pop();
    }
}
