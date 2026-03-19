using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoverEventComponent : MonoBehaviour
{

    public IEnumerator AddPlayerVelocity(SaveAndLoadLevel.Event e)
    {
        yield return new WaitForSeconds(0);

        PlayerMovementBasic.Instance.AddVelocity(transform.forward * float.Parse(e.doParameter));
    }

    public IEnumerator SetPlayerVelocity(SaveAndLoadLevel.Event e)
    {
        yield return new WaitForSeconds(0);

        PlayerMovementBasic.Instance.SetVelocity(transform.forward * float.Parse(e.doParameter));
    }

    public IEnumerator SetPlayerVelocity_Launch(SaveAndLoadLevel.Event e)
    {
        yield return new WaitForSeconds(0);

        PlayerMovementBasic.Instance.SetVelocity_Launch(transform.forward * float.Parse(e.doParameter));
    }
    public IEnumerator SetPlayerRotation()
    {
        yield return new WaitForSeconds(0);

        PlayerMovementBasic.Instance.transform.rotation = transform.rotation;
        SimpleSmoothMouseLook.Instance.ResetRotationX();  //make sure to do this or else the player camera angle will remain the same
    }

    public IEnumerator SetPlayerPosition()
    {
        yield return new WaitForSeconds(0);

        PlayerMovementBasic.Instance.transform.position = transform.position + (transform.up * ConfigMenuUIEvents.playerPositioningOffset);
    }


}
