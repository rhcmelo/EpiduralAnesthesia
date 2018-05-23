using UnityEngine;
using System.Collections;
using System;
using System.Runtime.InteropServices;
using System.Text;

/*
* Haptic functionnalities are loaded frokm the PluginImport Script
* To call a function, the code must be stated as below:
* PluginImport.function(Arg1,Arg2);
* In the case the functions is returning Array or required Arguments
* of Array type, it will be necessary tpo convert the data from/To IntPtr.
* To do so, call one of the functions define in the class ConverterClass.
*/

public class PluginImport  {

	// Funções Legacy (HD)
	[DllImport("hd.dll")]
	public static extern uint hdGetCurrentDevice();

	[DllImport("hd.dll")]
	public static extern void hdGetDoublev(ParameterName paramName, [Out] double[] value);

	// Parâmetros Legacy (HD)
	public enum ParameterName : ushort
	{
		HD_CURRENT_BUTTONS = 0x2000,
		HD_CURRENT_SAFETY_SWITCH = 0x2001,
		HD_CURRENT_INKWELL_SWITCH = 0x2002,
		HD_CURRENT_ENCODER_VALUES = 0x2010,
		HD_CURRENT_PINCH_VALUE = 0x2011,
		HD_LAST_PINCH_VALUE = 0x2012,
		HD_CURRENT_POSITION = 0x2050,
		HD_CURRENT_VELOCITY = 0x2051,
		HD_CURRENT_TRANSFORM = 0x2052,
		HD_CURRENT_ANGULAR_VELOCITY = 0x2053,
		HD_CURRENT_JACOBIAN = 0x2054,
		HD_CURRENT_JOINT_ANGLES = 0x2100,
		HD_CURRENT_GIMBAL_ANGLES = 0x2150,
		HD_LAST_BUTTONS = 0x2200,
		HD_LAST_SAFETY_SWITCH = 0x2201,
		HD_LAST_INKWELL_SWITCH = 0x2202,
		HD_LAST_ENCODER_VALUES = 0x2210,
		HD_LAST_POSITION = 0x2250,
		HD_LAST_VELOCITY = 0x2251,
		HD_LAST_TRANSFORM = 0x2252,
		HD_LAST_ANGULAR_VELOCITY = 0x2253,
		HD_LAST_JACOBIAN = 0x2254,
		HD_LAST_JOINT_ANGLES = 0x2300,
		HD_LAST_GIMBAL_ANGLES = 0x2350,
		HD_VERSION = 0x2500,
		HD_DEVICE_MODEL_TYPE = 0x2501,
		HD_DEVICE_DRIVER_VERSION = 0x2502,
		HD_DEVICE_VENDOR = 0x2503,
		HD_DEVICE_SERIAL_NUMBER = 0x2504,
		HD_DEVICE_FIRMWARE_VERSION = 0x2505,
		HD_MAX_WORKSPACE_DIMENSIONS = 0x2550,
		HD_USABLE_WORKSPACE_DIMENSIONS = 0x2551,
		HD_TABLETOP_OFFSET = 0x2552,
		HD_INPUT_DOF = 0x2553,
		HD_OUTPUT_DOF = 0x2554,
		HD_CALIBRATION_STYLE = 0x2555,
		HD_UPDATE_RATE = 0x2600,
		HD_INSTANTANEOUS_UPDATE_RATE = 0x2601,
		HD_NOMINAL_MAX_STIFFNESS = 0x2602,
		HD_NOMINAL_MAX_DAMPING = 0x2609,
		HD_NOMINAL_MAX_FORCE = 0x2603,
		HD_NOMINAL_MAX_CONTINUOUS_FORCE = 0x2604,
		HD_MOTOR_TEMPERATURE = 0x2605,
		HD_SOFTWARE_VELOCITY_LIMIT = 0x2606,
		HD_SOFTWARE_FORCE_IMPULSE_LIMIT = 0x2607,
		HD_FORCE_RAMPING_RATE = 0x2608,
		HD_NOMINAL_MAX_TORQUE_STIFFNESS = 0x2620,
		HD_NOMINAL_MAX_TORQUE_DAMPING = 0x2621,
		HD_NOMINAL_MAX_TORQUE_FORCE = 0x2622,
		HD_NOMINAL_MAX_TORQUE_CONTINUOUS_FORCE = 0x2623,
		HD_CURRENT_FORCE = 0x2700,
		HD_JOINT_ANGLE_REFERENCES = 0x2702,
		HD_CURRENT_JOINT_TORQUE = 0x2703,
		HD_CURRENT_GIMBAL_TORQUE = 0x2704,
		HD_LAST_FORCE = 0x2800,
		HD_LAST_JOINT_TORQUE = 0x2802,
		HD_LAST_GIMBAL_TORQUE = 0x2803,
		HD_USER_STATUS_LIGHT = 0x2900,
	}




	/*************************************************************/
	// Haptic Functions Import
	/*************************************************************/

	/*************************************************************/
	// Initialization and CleanUp Functions
	/*************************************************************/
	//Lets make our calls from the Plugin
	[DllImport ("ASimpleHapticPlugin")]
	public static extern bool InitHapticDevice(); 
	
	[DllImport ("ASimpleHapticPlugin")]
	public static extern bool HapticCleanUp(); 

	/*************************************************************/
	// Specify the Mode of Interaction
	/*************************************************************/

	/*
	* Mode = 0 Contact
	* Mode = 1 Manipulation - So objects will have a mass when handling them
	* Mode = 2 Custom Effect - So the haptic device simulate vibration and tangential forces as power tools
	* Mode = 3 Puncture - So the haptic device is a needle that puncture inside a geometry
	*/
	
	[DllImport ("ASimpleHapticPlugin")]
	public static extern void SetMode(int mode);
	
	[DllImport ("ASimpleHapticPlugin")]
	public static extern int GetMode();

	/*************************************************************/
	// Set the touchable Face(s)
	/*************************************************************/

	[DllImport ("ASimpleHapticPlugin")]
	public static extern void SetTouchableFace(IntPtr face);
	/*Argument must be converter from string to IntPtr
	*Accepted values: (if the function is not used - the plugin set the front face as default touchable face)
	* front
	* back
	* front_and_back
	*/
	
	/*************************************************************/
	// Workspace Functions
	/*************************************************************/
	
	[DllImport ("ASimpleHapticPlugin")]
	public static extern IntPtr SetWorkspacePosition(IntPtr position);
	
	[DllImport ("ASimpleHapticPlugin")]
	public static extern IntPtr SetWorkspaceSize(IntPtr size);
	
	//Encompass  SetWorkspacePosition +  SetWorkspaceSize + SetUpdateWorkspace
	[DllImport ("ASimpleHapticPlugin")]
	public static extern void SetWorkspace(IntPtr position,IntPtr size);
	
	[DllImport ("ASimpleHapticPlugin")]
	public static extern void UpdateWorkspace(float CameraAngleOnY);
	
	[DllImport ("ASimpleHapticPlugin")]
	public static extern IntPtr GetWorkspacePosition();
	
	[DllImport ("ASimpleHapticPlugin")]
	public static extern IntPtr GetWorkspaceSize();
	
	/*************************************************************/
	// Render the Haptics (All haptic threads) - to be used in the unity loop  
	/*************************************************************/

	[DllImport ("ASimpleHapticPlugin")]
	public static extern void RenderHaptic();
	
	/*************************************************************/
	// Haptic Device and Proxy Values
	/*************************************************************/
	
	[DllImport ("ASimpleHapticPlugin")]
	public static extern IntPtr GetDevicePosition();
	
	[DllImport ("ASimpleHapticPlugin")]
	public static extern IntPtr GetProxyPosition();
	
	[DllImport ("ASimpleHapticPlugin")]
	public static extern IntPtr GetProxyRight();
	
	[DllImport ("ASimpleHapticPlugin")]
	public static extern IntPtr GetProxyDirection();
	
	[DllImport ("ASimpleHapticPlugin")]
	public static extern IntPtr GetProxyTorque();
	
	[DllImport ("ASimpleHapticPlugin")]
	public static extern IntPtr GetProxyOrientation();
	
	/*************************************************************/
	// Set Haptic Objects Mesh and Matrix Transform
	/*************************************************************/
	
	[DllImport ("ASimpleHapticPlugin")]
	public static extern void SetObjectTransform(int objectId,IntPtr name, IntPtr transform);
	
	[DllImport ("ASimpleHapticPlugin")]
	public static extern void SetObjectMesh(int objectId, IntPtr vertices, IntPtr triangles, int verticesNum, int trianglesNum);

	/*************************************************************/
	// Set Haptic Objects information
	/*************************************************************/
	
	[DllImport ("ASimpleHapticPlugin")]
	public static extern IntPtr GetHapticObjectName(int ObjId);
	
	[DllImport ("ASimpleHapticPlugin")]
	public static extern int GetHapticObjectCount();
	
	[DllImport ("ASimpleHapticPlugin")]
	public static extern int GetHapticObjectFaceCount(int ObjId);
	
	/*************************************************************/
	// Set Haptic Properties for Haptic Objects
	/*************************************************************/

	[DllImport ("ASimpleHapticPlugin")]
	public static extern void SetHapticProperty(int ObjId, IntPtr type , float value); 

	//Haptic Property type

	/*basic properties*/	
		//stiffness													
		//damping											
		//staticFriction										
		//dynamicFriction

	/*Advanced properties for manipulation of objects*/
		//mass
		//fixed

	/*advanced properties for custom forces effects*/
		//tangentialStiffness
		//tangentialDamping

	/*Advanced properties for puncture effects */
		//popThrough
		//puncturedStaticFriction
		//puncturedDynamicFriction
	
	[DllImport ("ASimpleHapticPlugin")]
	public static extern void  SetStiffness(int ObjId, float stiffness);
	
	[DllImport ("ASimpleHapticPlugin")]
	public static extern  void  SetDamping(int ObjId, float damping);
	
	[DllImport ("ASimpleHapticPlugin")]
	public static extern  void  SetStaticFriction(int ObjId, float staticFriction);
	
	[DllImport ("ASimpleHapticPlugin")]
	public static extern  void  SetDynamicFriction(int ObjId, float dynamicFriction);
	
	[DllImport ("ASimpleHapticPlugin")]
	public static extern  void  SetTangentialStiffness(int ObjId, float tgStiffness);
	
	[DllImport ("ASimpleHapticPlugin")]
	public static extern  void  SetTangentialDamping(int ObjId, float tgDamping);
	
	[DllImport ("ASimpleHapticPlugin")]
	public static extern  void  SetPopThrough(int ObjId, float popThrough);
	
	[DllImport ("ASimpleHapticPlugin")]
	public static extern  void  SetPuncturedStaticFriction(int ObjId, float puncturedStaticFriction);
	
	[DllImport ("ASimpleHapticPlugin")]
	public static extern  void  SetPuncturedDynamicFriction(int ObjId, float puncturedDynamicFriction);

	[DllImport ("ASimpleHapticPlugin")]
	public static extern  void  SetMass(int ObjId,float mass);
	
	[DllImport ("ASimpleHapticPlugin")]
	public static extern  void  SetFixed(int ObjId,bool fix);

	[DllImport ("ASimpleHapticPlugin")]
	public static extern bool IsFixed(int ObjId);
	
	/*************************************************************/
	// Haptic Environmental Effects functions
	/*************************************************************/
	
	[DllImport ("ASimpleHapticPlugin")]
	public static extern bool SetEffect(IntPtr type,int effect_index, float gain, float magnitude, float duration,float frequency, IntPtr position, IntPtr direction);
	// Effects:		
	// constant			// vibrationMotor
	// spring			// vibrationContact
	// viscous			// tangentialForce
	// friction
	
	[DllImport ("ASimpleHapticPlugin")]
	public static extern bool StartEffect(int effect_index);
	
	[DllImport ("ASimpleHapticPlugin")]
	public static extern bool StopEffect(int effect_index);
	
	/*************************************************************/
	// Haptic Events Functions
	/*************************************************************/
	
	[DllImport ("ASimpleHapticPlugin")]
	public static extern  void LaunchHapticEvent();
	
	[DllImport ("ASimpleHapticPlugin")]
	public static extern bool GetButton1State();
	
	[DllImport ("ASimpleHapticPlugin")]
	public static extern bool GetButton2State();
	
	[DllImport ("ASimpleHapticPlugin")]
	public static extern IntPtr GetTouchedObjectName();
	
	[DllImport ("ASimpleHapticPlugin")]
	public static extern int GetTouchedObjectId();
	
	[DllImport ("ASimpleHapticPlugin")]
	public static extern IntPtr GetManipulatedObjectName();
	
	[DllImport ("ASimpleHapticPlugin")]
	public static extern int GetManipulatedObjectId();

	[DllImport ("ASimpleHapticPlugin")]
	public static extern bool GetContact();

	/*************************************************************/
	// Puncture Mode Specific Functions
	/*************************************************************/

	[DllImport ("ASimpleHapticPlugin")]
	public static extern void SetMaximumPunctureLenght(float maxPenetration);

	[DllImport ("ASimpleHapticPlugin")]
	public static extern bool GetPunctureState();

	[DllImport ("ASimpleHapticPlugin")]
	public static extern float GetPenetrationRatio();
	
	[DllImport ("ASimpleHapticPlugin")]
	public static extern IntPtr GetFirstScpPt();

    [DllImport("ASimpleHapticPlugin")]
    public static extern void SetPunctureDirection(IntPtr punctureVectorDirection);
	
	[DllImport ("ASimpleHapticPlugin")]
	public static extern IntPtr GetPunctureDirection();

	[DllImport ("ASimpleHapticPlugin")]
	public static extern void SetPunctureLayers(int layerNb, IntPtr[] nameObjects , IntPtr layerDepth);

	/*************************************************************/
	
}
