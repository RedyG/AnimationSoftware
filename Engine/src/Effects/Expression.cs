using Engine.Core;
using Engine.Attributes;
using Engine.UI;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Reflection.Metadata;
using System.Reflection;
using Newtonsoft.Json;

namespace Engine.Effects
{
    [Description(Category = "Utility")]
    public class Expression : Effect
    {
        [JsonProperty]
        private Guid _id;

        public Parameter<string> Code { get; } = new Parameter<string>("", false, true);
        public Parameter<object> Value { get; } = new Parameter<object>(new object(), false, false);

        private readonly ExpressionCodeUI _valueUI = new ExpressionCodeUI();

        protected override ParameterList InitParameters() => new ParameterList(
            new("Code", Code),
            new("Value", Value)
        );

        [UIMethod(1)]
        private void CompileButton()
        {

        }

        public Expression()
        {
            _id = Guid.NewGuid();

            Code.CustomUI = _valueUI;
            Value.ValueGetter += ValueGetter;
        }

        private object ValueGetter(object? sender, ValueGetterEventArgs args)
        {
            var result = _valueUI.Script();
            result.Wait();
            return result.Result;
        }
    }

    public class ExpressionCodeUI: IParameterUI<string>
    {
        public ScriptRunner<object> Script = CSharpScript.Create("2f", ScriptOptions.Default).CreateDelegate();

        public UILocation Location => UILocation.Under;

        public void Draw(Parameter<string> parameter)
        {
            var value = parameter.BeginValueChange();
            ImGui.InputTextMultiline("", ref value, 99999, new Vector2(ImGui.GetContentRegionAvail().X, 400));
            parameter.EndValueChange(value);
            if(ImGui.Button("Open in VSCode"))
            {
                using (Process process = new Process())
                {
                    process.StartInfo.UseShellExecute = true;
                    process.StartInfo.FileName = "code";
                    process.StartInfo.Arguments = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + $"\\misc\\expressions\\{"a"}.cs";
                    process.StartInfo.CreateNoWindow = true;
                    process.Start();
                }
            }
            if (ImGui.Button("Compile"))
            {
                var options = ScriptOptions.Default
                    .WithImports("Engine.Core")
                    .WithReferences(typeof(App).Assembly)
                    .WithOptimizationLevel(Microsoft.CodeAnalysis.OptimizationLevel.Release);

                var script = CSharpScript.Create(parameter.Value, options);
                Script = script.CreateDelegate();
            }
        }

    }
}
