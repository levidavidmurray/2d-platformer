using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DebugUI : MonoBehaviour
{
    public TMP_Text currentStateText;
    public TMP_Text currentVelocityText;

    public void OnStateChange(PlayerState state) {
        currentStateText.SetText(state.GetType().Name);
    }

    public void OnPlayerVelocityChange(Vector2 velocity) {
        currentVelocityText.SetText(velocity.ToString());
    }


}
