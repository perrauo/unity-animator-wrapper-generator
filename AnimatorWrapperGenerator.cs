#if UNITY_EDITOR

using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Text;
using System.IO;
using System.Linq;

namespace Cirrus.Animations
{
    [CreateAssetMenu(menuName = "Cirrus/Animations/Animator Wrapper")]
    public class AnimatorWrapperGenerator : ScriptableObject
    {
        [SerializeField]
        private UnityEditor.Animations.AnimatorController _animator;

        [SerializeField]
        private string _name;

        [SerializeField]
        private string _namespace;

        private const int kSpacesPerIndentLevel = 4;

        public string FormatField(string field)
        {
            return field.ToLower().Replace(" ", "");
        }

        public string FormatProperty(string property)
        {
            string res = property.First().ToString().ToUpper() + property.Substring(1);
            return res.Replace(" ", "");
        }

        public string GetTypeString1(AnimatorControllerParameterType type)
        {
            switch (type)
            {
                case AnimatorControllerParameterType.Bool:
                    return "bool";
                case AnimatorControllerParameterType.Float:
                    return "float";
                case AnimatorControllerParameterType.Int:
                    return "int";
                case AnimatorControllerParameterType.Trigger:
                    return "void";
                default:
                    return "";
            }
        }

        public string GetTypeString2(AnimatorControllerParameterType type)
        {
            switch (type)
            {
                case AnimatorControllerParameterType.Bool:
                    return "Bool";
                case AnimatorControllerParameterType.Float:
                    return "Float";
                case AnimatorControllerParameterType.Int:
                    return "Integer";
                case AnimatorControllerParameterType.Trigger:
                    return "Trigger";
                default:
                    return "";

            }
        }

  
        public string FormatClass(string cls)
        {
            return $"{cls}AnimatorWrapper";
        }

        public string FormatInterface(string cls)
        {
            return "I"+FormatClass(cls);
        }


        public string FormatAnimationEnum(string cls)
        {
            return $"{cls}Animation";
        }

        public string FormatAnimation(string cls)
        {
            return cls;
        }

        public string FormatClip(string state)
        {
            return state.Replace("_", ".");
        }

        //public string FormatLayer(string cls)
        //{
        //    return cls.Remove(;
        //}

        private const string _stateSpeedValuesString = "_stateSpeedValues";

        public string GenerateWrapperCode()
        {
            if (string.IsNullOrEmpty(_name))
            {
            }

            var writer = new Writer
            {
                buffer = new StringBuilder()
            };


            // Usings.
            writer.WriteLine("using System;");
            writer.WriteLine("using System.Collections;");
            writer.WriteLine("using System.Collections.Generic;");
            writer.WriteLine("using UnityEngine;");
            writer.WriteLine("");

            // Begin namespace.

            writer.WriteLine($"namespace {_namespace}");
            writer.BeginBlock();

            // Begin Animation Enum
            writer.WriteLine($"public enum {FormatAnimationEnum(_name)}");
            writer.BeginBlock();
            foreach (var layer in _animator.layers)
            {
                foreach (var state in layer.stateMachine.states)
                {                    
                    writer.WriteLine($"{FormatAnimation(state.state.name)}={state.state.nameHash},");
                }
            }
            writer.EndBlock();


            // Begin interface.
            writer.WriteLine($"public interface {FormatInterface(_name)}");
            writer.BeginBlock();
            writer.WriteLine($"float GetStateSpeed({FormatAnimationEnum(_name)} state);");
            writer.WriteLine($"void Play({FormatAnimationEnum(_name)} animation, float normalizedTime);");
            writer.WriteLine($"void Play({FormatAnimationEnum(_name)} animation);");
            // Properties
            foreach (var param in _animator.parameters)
            {
                string endline = param.type == AnimatorControllerParameterType.Trigger ?
                    $"void {FormatProperty(param.name)}();" :
                    $"{GetTypeString1(param.type)} {FormatProperty(param.name)} {{ set; }}";
                writer.WriteLine(endline);
            }

            for(int i = 0; i < _animator.layers.Length; i++)
            {
                writer.WriteLine($"float {FormatProperty(_animator.layers[i].name)}LayerWeight{{set;}}");
            }

            writer.EndBlock();


            // Begin class.
            writer.WriteLine($"public class {FormatClass(_name)} : {FormatInterface(_name)}");
            writer.BeginBlock();


            writer.WriteLine($"private Animator _animator;");
            writer.WriteLine($"private Dictionary<{FormatAnimationEnum(_name)},float> {_stateSpeedValuesString} = new Dictionary<{FormatAnimationEnum(_name)},float>();");

            // Play method
            writer.WriteLine($"public void Play({FormatAnimationEnum(_name)} animation, float normalizedTime)");
            writer.BeginBlock();
            writer.WriteLine("if(_animator != null)_animator.Play((int)animation, -1, normalizedTime);");
            writer.EndBlock();

            writer.WriteLine($"public void Play({FormatAnimationEnum(_name)} animation)");
            writer.BeginBlock();
            writer.WriteLine("if(_animator != null)_animator.Play((int)animation);");
            writer.EndBlock();

            // Properties
            foreach (var param in _animator.parameters)
            {
                string endline = param.type == AnimatorControllerParameterType.Trigger ?
                    $"(){{ if(_animator != null)_animator.Set{ GetTypeString2(param.type)}({Animator.StringToHash(param.name)}); }}" :
                    $"{{ set {{ if(_animator != null)_animator.Set{ GetTypeString2(param.type)}({Animator.StringToHash(param.name)}, value); }} }}";
                writer.WriteLine($"public {GetTypeString1(param.type)} {FormatProperty(param.name)}"  + endline);
            }

            for (int i = 0; i < _animator.layers.Length; i++)
            {
                writer.WriteLine($"public float {FormatProperty(_animator.layers[i].name)}LayerWeight{{set {{ if(_animator != null) _animator.SetLayerWeight({i},value);}} }}");
            }

            // Constructor
            writer.WriteLine($"public {FormatClass(_name)}(Animator animator)");
            writer.BeginBlock();
            writer.WriteLine("_animator = animator;");
            foreach (var layer in _animator.layers)
            {
                foreach (var state in layer.stateMachine.states)
                {
                    writer.WriteLine($"{_stateSpeedValuesString}.Add({FormatAnimationEnum(_name)}.{FormatAnimation(state.state.name)},{state.state.speed});");
                }
            }
            writer.EndBlock();

            writer.WriteLine($"public float GetStateSpeed({FormatAnimationEnum(_name)} state)");
            writer.BeginBlock();
            writer.WriteLine($"if({_stateSpeedValuesString}.TryGetValue(state, out float res)) return res;");
            writer.WriteLine("return -1f;");
            writer.EndBlock();

            writer.WriteLine($"public float GetClipLength({FormatAnimationEnum(_name)} state)");
            writer.BeginBlock();
            writer.WriteLine("return -1f;");
            writer.EndBlock();

            // End class
            writer.EndBlock();

            // End namespace
            writer.EndBlock();


            return writer.buffer.ToString();
        }

        private struct Writer
        {
            public StringBuilder buffer;
            public int indentLevel;

            public void BeginBlock()
            {
                WriteIndent();
                buffer.Append("{\n");
                ++indentLevel;
            }

            public void EndBlock()
            {
                --indentLevel;
                WriteIndent();
                buffer.Append("}\n");
            }

            public void WriteLine()
            {
                buffer.Append('\n');
            }

            public void WriteLine(string text)
            {
                WriteIndent();
                buffer.Append(text);
                buffer.Append('\n');
            }

            public void Write(string text)
            {
                buffer.Append(text);
            }

            public void WriteIndent()
            {
                for (var i = 0; i < indentLevel; ++i)
                {
                    for (var n = 0; n < kSpacesPerIndentLevel; ++n)
                        buffer.Append(' ');
                }
            }
        }

        public void GenerateAnimatorWrapperFile()
        {
            string filePath = Path.GetDirectoryName(AssetDatabase.GetAssetPath(this));

            filePath += $"/{FormatClass(_name)}.cs";

            // Generate code.
            var code = GenerateWrapperCode();

            // Write.
            File.WriteAllText(filePath, code);
        }

    }



    [CustomEditor(typeof(AnimatorWrapperGenerator))]
    public class SomeScriptEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            AnimatorWrapperGenerator myScript = (AnimatorWrapperGenerator)target;
            if (GUILayout.Button("Generate"))
            {
                myScript.GenerateAnimatorWrapperFile();
            }
        }
    }
}


#endif