using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class JoystickPlayerExample : MonoBehaviour
{
    public float speed;
    private float speedMath = 500f;
    private float speedMathVerticalStable = 500f;
    private float speedMathHorizontalStable = 100f;

    public VariableJoystick variableJoystick;
    public Rigidbody rb;
    public Transform tr;
    public Transform trParent;
    private Quaternion defaultQuartion;
    private const int DiffAngleForSet = 90;// 45;
    private const float SpeedSwing = 10f;
    private const int EurlerDegreeForStop = 40;
    private const int EurlerDegreeForStartStoppingBeforeStop = 10;
    private const float ProportionHeightWidthDefault = 2;
    //public Camera camera;


    //private Quaternion lastQuitQuartion;

    float angleY = 0f;
    float angleX = 0f;
    int Rotations = 0;
    bool isRotationCompleteRound = false;

    bool isRotationFinishing = false;

    bool isStartRotating = false;

    bool isSwinging = false;
    bool isSwingingLeft = false;

    private void Awake()
    {
        defaultQuartion = transform.rotation;
        //lastQuitQuartion = transform.rotation;
        isSwinging = true;
        rb.isKinematic = true;
        //Debug.Log(Screen.height/ Screen.width);
        //float prop = Screen.height / Screen.width;
        //float coefDistanceFromCamera = prop / ProportionHeightWidthDefault;

        //trParent.localPosition = new Vector3(trParent.localPosition.x,
        //    trParent.localPosition.y,
        //    (trParent.localPosition.z - 2.4f)  * coefDistanceFromCamera + 2.4f);

        textTest1.text = speedMath.ToString();
        textTest2.text = speedMathHorizontalStable.ToString();
        textTest3.text = speedMathVerticalStable.ToString();
    }

    static bool IsUp = false;
    static bool IsDown = false;

    public static void PointUp()
    {
        //Debug.Log("OnUp");
        IsUp = true;
        IsDown = false;
    }

    public static void PointDown()
    {
        //Debug.Log("PointDown");
        //IsUp = false;
        IsDown = true;
    }

    public readonly int R_TEST = 0; // 0 - Hard, 1 - Swing+Rot, 2 - Math by button

    public void FixedUpdate()
    {
        //CountRounds();
        switch (R_TEST)
        {
            case 0:
                RotationControlHard();
                break;
            case 1:
                if (isSwinging)
                {
                    ControlSwing();
                }
                RotationControl();
                break;
            case 2:
                RotationMathControl();
                break;
            default:
                break;
        }
        //if (isRotationFinishing)
        //{
        //    FinishingRotaion();
        //}
    }

    void ControlSwing()
    {
        float magnitude = 0.01f;
        float speedS = SpeedSwing;
        if (!rb.isKinematic)
            speedS = speedS * 2f;
        //rb.isKinematic = false;
        if (rb.angularVelocity.magnitude < magnitude)
        {
            isSwingingLeft = !isSwingingLeft;
            MakeSingelSwing(speedS);
            //Debug.Log("SwitchSwing");
        }
    }

    bool RoundChecked = false;

    private const float EulerDegreeForCountRounds = 340f;

    void CountRounds()
    {
        float y = tr.rotation.eulerAngles.y;
        if (!RoundChecked && y >= EulerDegreeForCountRounds)
        {
            
            RoundChecked = true;
            
        }
        if (RoundChecked && y < EulerDegreeForCountRounds)
        {
            Rotations++;
            RoundChecked = false;
            Debug.Log(Rotations);
        }
            
    }

    void FinishingRotaion()
    {
        transform.rotation = Quaternion.Lerp(transform.rotation, defaultQuartion, Time.fixedDeltaTime * 3f);
        if (defaultQuartion.eulerAngles.y == (int)AngleSet.First)
        {
            if (Math.Abs((int)transform.rotation.eulerAngles.y - (int)defaultQuartion.eulerAngles.y) < 2)
            {
                rb.isKinematic = true;
                isRotationFinishing = false;
                if (variableJoystick.Horizontal == 0)
                {
                    isStartRotating = false;
                }
                transform.rotation = defaultQuartion;
                MakeSingelSwing(SpeedSwing);
            }
        }
        else
        {
            if (Math.Abs((int)transform.rotation.eulerAngles.y - (int)defaultQuartion.eulerAngles.y) < 45)
            {
                Debug.Log("getNextBiggerAngle");
                defaultQuartion = getNextBiggerAngle();
            }
        }

    }

    void RotationControl()
    {
        if (speed > 0)
        {
            angleY = tr.rotation.eulerAngles.y;
        }
        if (variableJoystick.Horizontal != 0)
        {
            rb.isKinematic = false;
            isStartRotating = true;
            isRotationFinishing = false;
            isSwinging = false;
        }
        if (variableJoystick.Horizontal == 0 && !isRotationFinishing && isStartRotating && !isSwinging)
        {
            float magnitude = 1.7f;
            //if (isSwinging)
            //{
            //    magnitude = 0.01f;
            //}
            isRotationFinishing = true ? rb.angularVelocity.magnitude < magnitude : false;
            if (isRotationFinishing)
            {
                //defaultQuartion = getClosestAngle(transform.rotation);
                defaultQuartion = getClosestBiggerAngle(transform.rotation);
                //defaultQuartion = getStartAngle(transform.rotation);
                Debug.Log(defaultQuartion.eulerAngles.y);

            }
            //Debug.Log(defaultQuartion);
        }
        //isRotationFinishing = true? variableJoystick.Horizontal == 0 && rb.angularVelocity.magnitude < 0.5f : false;

        //if (isRotationFinishing)
        //{
        //    transform.rotation = Quaternion.Lerp(transform.rotation, defaultQuartion, Time.fixedDeltaTime * 3f);
        //    //if (Math.Abs((int)transform.rotation.eulerAngles.y - (int)defaultQuartion.eulerAngles.y) < 2)
        //    //{
        //    if (defaultQuartion.eulerAngles.y == (int)AngleSet.First)
        //    {
        //        if (Math.Abs((int)transform.rotation.eulerAngles.y - (int)defaultQuartion.eulerAngles.y) < 2)
        //        {
        //            rb.isKinematic = true;
        //            isRotationFinishing = false;
        //            if (variableJoystick.Horizontal == 0)
        //            {
        //                isStartRotating = false;
        //            }
        //            transform.rotation = defaultQuartion;
        //            MakeSingelSwing(SpeedSwing);
        //        }
        //    }
        //    else
        //    {
        //        if (Math.Abs((int)transform.rotation.eulerAngles.y - (int)defaultQuartion.eulerAngles.y) < 45)
        //        {
        //            Debug.Log("getNextBiggerAngle");
        //            defaultQuartion = getNextBiggerAngle();
        //        }
        //    }
                
        //    //}
        //}
        else if (!isSwinging)
        {
            //Vector3 direction = Vector3.right * variableJoystick.Horizontal;
            //rb.AddTorque(direction * speed * Time.fixedDeltaTime, ForceMode.VelocityChange);
            rb.AddTorque(new Vector3(0f, -variableJoystick.Horizontal * speed * Time.fixedDeltaTime, 0f), ForceMode.VelocityChange);
            //tr.RotateAround(new Vector3(0f, tr.position.y, 0f), -variableJoystick.Horizontal * speed * Time.fixedDeltaTime);
            //lastQuitQuartion = transform.rotation;
        }
    }

    float previosRot = 0f;
    float currentRot = 0f;
    float deltaRot = 0f;
    private static bool isCoroutineStarted = false;


    IEnumerator StartStableVerticalCube()
    {
        //isCoroutineStarted = true;
        //float x_start = WrapAngle(trParent.localRotation.eulerAngles.x);
        //float rad_pi = trParent.localRotation.x*Mathf.PI;
        float x_start = 180f * trParent.localRotation.x;
        //float x_start = trParent.localRotation.eulerAngles.x;
        Debug.Log("x_start: " + x_start);
        //Debug.Log("trParent.localRotation.eulerAngles.x: " + trParent.localRotation.eulerAngles.x);
        //Debug.Log("trParent.localRotation.x: " + trParent.localRotation.x);
        //Debug.Log("angle: " + angle);
        float direction = 1f;
        if (trParent.localRotation.x > 0f)
        {
            direction = -1f;
        }
        int countSwing = 2;
        int timeSecStartSwing = (int)Time.timeSinceLevelLoad;
        int maxCountSecondForSwing = 8; 
        while (true)
        {
            //trParent.localRotation = Quaternion.Euler(trParent.localRotation.eulerAngles.x + Time.fixedDeltaTime * speedMath * 2f * direction, 0f, 0f);
            trParent.localRotation = new Quaternion(trParent.localRotation.x + (Time.fixedDeltaTime * speedMathVerticalStable * direction)/180f, 0f, 0f, trParent.localRotation.w);
            yield return new WaitForFixedUpdate();
            if (direction < 0f && trParent.localRotation.x * 180f < -(Math.Abs(x_start) / 1.5f)
                ||
                direction > 0f && trParent.localRotation.x * 180f > Math.Abs(x_start) / 1.5f
                )
            {
                if ((int)x_start == 0)
                {
                    break;
                }
                direction *= -1f;
                x_start = x_start / 2f;
                countSwing--;
                if (countSwing <= 1)
                {
                    x_start = 0f;
                }
            }
            
            if (timeSecStartSwing + maxCountSecondForSwing < (int)Time.timeSinceLevelLoad)
            {
                break;
            }
        }
        trParent.localRotation = Quaternion.Euler(Vector3.zero);
        yield return null;
        //isCoroutineStarted = false;
    }

    IEnumerator StartRotateAfterScrollByMath(float speedCur)
    {
        isCoroutineStarted = true;
        speedCurrent = speedCur;
        //Debug.Log(speedCurrent);
        bool needStop = false;
        while (!needStop)
        {
            needStop = RotationMathControl();
            yield return new WaitForFixedUpdate();
        }
        isCoroutineStarted = false;
        yield return null;
    }

    private static float WrapAngle(float angle)
    {
        angle %= 360;
        if (angle > 180)
            return angle - 360;

        return angle;
    }

    void RotationControlHard()
    { 
        if (IsUp)
        {
            rb.isKinematic = false;
            //Debug.Log("fixedDragPositionXArray.Count: " + fixedDragPositionXArray.Count);
            Debug.Log("GetDeltaXForLastFrames: " + GetDeltaXForLastFrames(10));
            //trParent.localRotation = Quaternion.Euler(0f, 0f, 0f);

            //rb.AddTorque(new Vector3(0f, -GetDeltaXForLastFrames(10) * 20, 0f), ForceMode.VelocityChange);
            //rb.angularDrag = 1.0f;

            StartCoroutine(StartRotateAfterScrollByMath(-GetDeltaXForLastFrames(10) * 1000));
            StartCoroutine(StartStableVerticalCube());
            
            IsUp = false;
            fixedDragPositionXArray.Clear();
            //rb.drag = 0.1f;
            //Debug.Log(deltaRot);
        }
        if(IsDown)
        {
            //rb.isKinematic = true;
            //rb.angularVelocity = Vector3.zero;
            if (Math.Abs(variableJoystick.Horizontal) > 0.001f || Math.Abs(variableJoystick.Vertical) > 0.001f)
            {
                //if (isCoroutineStarted)
                //{
                //    StopAllCoroutines();
                //    trParent.localRotation = Quaternion.Euler(Vector3.zero);
                //    angleX = trParent.localRotation.eulerAngles.x;
                //}
                if (Math.Abs(variableJoystick.Horizontal) > 0.001f)
                {
                    tr.localRotation = Quaternion.Euler(0f, angleY - variableJoystick.Horizontal * speedMath, 0f);
                }
                if (Math.Abs(variableJoystick.Vertical) > 0.001f)
                {
                    
                    trParent.localRotation = Quaternion.Euler(angleX + variableJoystick.Vertical * speedMath, 0f, 0f);
                }
                FillDragPositionXArray(variableJoystick.Horizontal);
                //rb.isKinematic = true;
                //rb.angularDrag = 10;

            }
            else
            {
                angleY = tr.localRotation.eulerAngles.y;
                angleX = trParent.localRotation.eulerAngles.x;
            }
        }
        //Debug.Log(variableJoystick.Horizontal);

    }

    List<float> fixedDragPositionXArray = new List<float>();

    void FillDragPositionXArray(float valueX)
    {
        if (fixedDragPositionXArray.Count > 0)
            fixedDragPositionXArray.Insert(0, valueX);
        else
            fixedDragPositionXArray.Add(valueX);
        
    }

    float GetDeltaXForLastFrames(int frameCounts)
    {
        if (fixedDragPositionXArray.Count == 0)
        {
            return 0f;
        }
        if (fixedDragPositionXArray.Count >= frameCounts)
            //return fixedDragPositionXArray.Take(frameCounts).Average();
            return fixedDragPositionXArray[0] - fixedDragPositionXArray[frameCounts-1];
        else
            return fixedDragPositionXArray.Average();
    }

    private float speedCurrent = 0f;

    bool RotationMathControl()
    {
        if (speedCurrent == 0)
        {
            return true;
        }
        //MathAccelerate();
        bool needStop = MathSlow();
        if (!needStop)
        {
            tr.localRotation = Quaternion.Euler(0f, tr.localRotation.eulerAngles.y + Time.fixedDeltaTime * speedCurrent, 0f);
        }
        else
        {
            needStop = true;
        }
        return needStop;
    }

    void MathAccelerate()
    {
        if (Rotations == 0 && (int)speedCurrent < (int)speedMathHorizontalStable)
        {
            if ((int)speedCurrent == 0)
            {
                speedCurrent = 10f;
            }
            else
            {
                speedCurrent *= 1.05f;
            }
            Debug.Log("speedCurrent: " + (int)speedCurrent);
        }
    }

    bool isNeedToStop = false;
    bool isStopping = false;

    bool MathSlow()
    {
        if (speedCurrent >= speedMathHorizontalStable)
            isNeedToStop = false;
        else
            isNeedToStop = true;

        if (isNeedToStop)
        {
            if ((int)WrapAngle(tr.localRotation.eulerAngles.y) <= EurlerDegreeForStop
                && (int)WrapAngle(tr.localRotation.eulerAngles.y) > EurlerDegreeForStop - EurlerDegreeForStartStoppingBeforeStop)
            {
                speedCurrent = speedMathHorizontalStable / (EurlerDegreeForStartStoppingBeforeStop - (EurlerDegreeForStop - (int)WrapAngle(tr.localRotation.eulerAngles.y)));
                isStopping = true;
            }
            else if (speedCurrent >= speedMathHorizontalStable)
            {
                speedCurrent *= 0.99f;
            }
        }
        else
        {
            if (speedCurrent >= speedMathHorizontalStable)
                speedCurrent *= 0.992f;
        }

        if (isStopping && (int)WrapAngle(tr.localRotation.eulerAngles.y) >= EurlerDegreeForStop)
        {
            //Debug.Log("2: " + tr.rotation.eulerAngles.y);
            //Debug.Log("3: " + (int)WrapAngle(tr.rotation.eulerAngles.y));
            tr.localRotation = Quaternion.Euler(0f, EurlerDegreeForStop, 0f);
            speedCurrent = 0f;
            isStopping = false;
            return true;
            
        }
        return false;
    }

    #region Quaternions

    Quaternion getClosestAngle(Quaternion currnetRotation)
    {
        foreach (AngleSet angle in Enum.GetValues(typeof(AngleSet)))
        {
            int diff = (int)angle - (int)currnetRotation.eulerAngles.y;
            
            if (Math.Abs(diff) < DiffAngleForSet)
            {
                   return Quaternion.Euler(0, (int)angle, 0);
            }
            else
            {
                if ((int)angle + 360 - (int)currnetRotation.eulerAngles.y < DiffAngleForSet)
                {
                    return Quaternion.Euler(0, (int)angle, 0);
                }
            }
        }
        return Quaternion.Euler(0, (int)AngleSet.First, 0);
    }

    Quaternion getClosestBiggerAngle(Quaternion currnetRotation)
    {
        foreach (AngleSet angle in Enum.GetValues(typeof(AngleSet)))
        {
            int diff = (int)angle - (int)currnetRotation.eulerAngles.y;
            //Debug.Log("0 Diff: " + diff);
            if (diff > 0 && diff < DiffAngleForSet)
            {
                //Debug.Log("1 Diff: " + diff);
                return Quaternion.Euler(0, (int)angle, 0);
            }
            else
            {
                //if ((int)angle + 360 - (int)currnetRotation.eulerAngles.y < DiffAngleForSet)
                //{
                //    return Quaternion.Euler(0, (int)angle, 0);
                //}
            }
        }
        //Debug.Log("2 Diff: Default");
        return Quaternion.Euler(0, (int)AngleSet.First, 0);
    }

    Quaternion getNextBiggerAngle()
    {
        foreach (AngleSet angle in Enum.GetValues(typeof(AngleSet)))
        {
            int diff = (int)angle - (int)defaultQuartion.eulerAngles.y;
            Debug.Log("0 Diff: " + diff);
            if (diff > 0)
            {
                Debug.Log("1 Diff: " + diff);
                return Quaternion.Euler(0, (int)angle, 0);
            }
        }
        Debug.Log("2 Diff: Default");
        return Quaternion.Euler(0, (int)AngleSet.First, 0);
    }

    Quaternion getStartAngle(Quaternion currnetRotation)
    {
        return Quaternion.Euler(0, (int)AngleSet.First, 0);
        //return Quaternion.Euler(0, 359, 0);
    }

    #endregion

    public void MakeSingelSwing(float speedS)
    {
        int directionCoef = 1;
        if (isSwingingLeft)
            directionCoef = -1;

        rb.isKinematic = false;
        isStartRotating = true;
        isRotationFinishing = false;
        rb.AddTorque(new Vector3(0f, directionCoef * speedS * Time.fixedDeltaTime, 0f), ForceMode.VelocityChange);
        isSwinging = true;
    }

    public Color topColor = Color.blue;
    public Color bottomColor = Color.white;
    public int gradientLayer = 7;

    #region TestButtons

    public Text textTest1;
    float speedMath_temp = 500f;
    public void ButtonClick1(float speedS)
    {
        if (speedMath_temp > 500f)
        {
            speedMath_temp += 200f;
        }
        else
        {
            speedMath_temp += 100f;
        }
        
        if (speedMath_temp > 2100f)
        {
            speedMath_temp = 150f;
        }
        speedMath = speedMath_temp;
        textTest1.text = speedMath.ToString();
        //MakeSingelSwing(speedS);
    }

    public Text textTest2;
    float speedMathHorizontalStable_temp = 100f;
    public void ButtonClick2()
    {
        speedMathHorizontalStable_temp += 50f;
        if (speedMathHorizontalStable_temp > 500f)
        {
            speedMathHorizontalStable_temp = 50f;
        }
        speedMathHorizontalStable = speedMathHorizontalStable_temp;
        textTest2.text = speedMathHorizontalStable.ToString();

        //if (speedCurrent > 0)
        //{
        //    return;
        //}
        //Rotations = 0;
        //System.Random r = new System.Random();
        //speedCurrent = speedMathHorizontalStable * ((float)r.Next(1,30))/ (float)r.Next(2, 5);
        ////speedCurrent = 700f;
        //RoundChecked = false;
        //Debug.Log(speedCurrent);

    }

    public Text textTest3;
    float speedMathVerticalStable_temp = 500f;
    public void ButtonClick3()
    {
        speedMathVerticalStable_temp += 50f;
        if (speedMathVerticalStable_temp > 1000f)
        {
            speedMathVerticalStable_temp = 50f;
        }
        speedMathVerticalStable = speedMathVerticalStable_temp;
        //isRotationFinishing = true;

        textTest3.text = speedMathVerticalStable.ToString();
    }

    #endregion
}