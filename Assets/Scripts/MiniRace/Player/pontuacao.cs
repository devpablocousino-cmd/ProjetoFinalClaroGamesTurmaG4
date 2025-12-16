using UnityEngine;

public class Pontuacao : MonoBehaviour
{
    private int pontos;
    private int checkpoints;

    // 🔥 Estado salvo do último checkpoint
    private Vector3 ultimoCheckpointPos;
    private Quaternion ultimoCheckpointRot;
    private bool temCheckpoint = false;

    // -----------------------------
    // Pontos
    // -----------------------------
    public void AdicionarPontos(int quantidade)
    {
        pontos += quantidade;
    }

    public void ZerarPontos()
    {
        pontos = 0;
    }

    public int GetPontos()
    {
        return pontos;
    }

    // -----------------------------
    // Checkpoints
    // -----------------------------
    public void AdicionarCheckpoint()
    {
        checkpoints += 1;
    }

    public void ZerarCheckpoints()
    {
        checkpoints = 0;
        temCheckpoint = false;
    }

    public int GetCheckpoints()
    {
        return checkpoints;
    }

    // -----------------------------
    // Último checkpoint (CORRIGIDO)
    // -----------------------------

    /// <summary>
    /// Salva a posição e rotação do checkpoint.
    /// </summary>
    public void SetUltimoCheckpoint(Transform checkpoint)
    {
        if (checkpoint == null)
            return;

        ultimoCheckpointPos = checkpoint.position;
        ultimoCheckpointRot = checkpoint.rotation;
        temCheckpoint = true;
    }

    /// <summary>
    /// Retorna true se existir checkpoint salvo.
    /// </summary>
    public bool TemUltimoCheckpoint()
    {
        return temCheckpoint;
    }

    /// <summary>
    /// Aplica o último checkpoint em um Transform alvo.
    /// </summary>
    public void AplicarUltimoCheckpoint(Transform alvo)
    {
        if (!temCheckpoint || alvo == null)
            return;

        alvo.SetPositionAndRotation(
            ultimoCheckpointPos,
            ultimoCheckpointRot
        );
    }

    public void ObterUltimoCheckpoint(out Vector3 pos, out Quaternion rot)
    {
        pos = ultimoCheckpointPos;
        rot = ultimoCheckpointRot;
    }

}
