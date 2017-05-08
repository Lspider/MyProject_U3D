using UnityEngine;
using System.Collections;

public class TapBehaviour : MonoBehaviour {
    private int mTapCount = 0;
    private float mTimeSinceLastTap = 0;
    private const float DOUBLE_TAP_MAX_DELAY = 0.5f;

    // Use this for initialization
    void Start () {
	
	}

    int HandleTap()
    {
        int nTapCount = 0;
        if (mTapCount == 1)
        {
            mTimeSinceLastTap += Time.deltaTime;
            if (mTimeSinceLastTap > DOUBLE_TAP_MAX_DELAY)
            {
                // too late for double tap, 
                // we confirm it was a single tap
                nTapCount = mTapCount;

                // reset touch count and timer
                mTapCount = 0;
                mTimeSinceLastTap = 0;
            }
        }
        else if (mTapCount == 2)
        {
            // we got a double tap
            nTapCount = mTapCount;

            // reset touch count and timer
            mTimeSinceLastTap = 0;
            mTapCount = 0;
        }

        if (Input.GetMouseButtonUp(0))
        {
            mTapCount++;
            if (mTapCount == 1)
            {
                nTapCount = mTapCount;
            }
        }

        return mTapCount;
    }
}
