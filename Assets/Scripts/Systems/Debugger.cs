using UnityEngine;

public class Debugger : IDebugger
{
    //! /<summary>
    //! ------------------------------------------------------------------------------------------------------------------
    //! --- Para esta classe funcionar, é preciso criar uma variável bool no outro script (nome padrão "IsDebugging"), ---
    //! --- depois no método Awake ou OnEnable criar uma nova instância do Debugger e criar uma event Action no script. --
    //! --- Assinar essa event Action com o método do Debugger chamado "OnDebuggingChanged". E no OnDisable do script  ---
    //! --- remover a assinatura do método "OnDebuggingChanged".                                                       ---
    //! ------------------------------------------------------------------------------------------------------------------
    //! </summary>

    private bool isDebugging;

    public void Log(string message)
    {
        if (isDebugging)
            Debug.Log(message);
    }

    public void Log(object obj)
    {
        if (isDebugging)
            Debug.Log(obj);
    }

    //! --- Atualiza o valor de "isDebugging" ---
    public void OnDebuggingChanged(bool _newValue)
    {
        isDebugging = _newValue;
    }
}

public interface IDebugger
{
    public void Log(string message);
    public void Log(object obj);
    public void OnDebuggingChanged(bool newValue);
}
