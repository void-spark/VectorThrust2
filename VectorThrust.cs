// ALWAYS CHECK FOR UPDATES
// to update, simply load it from the workshop tab again (no need to actually go to the workshop page)

// weather or not dampeners are on when you start the script
public bool dampeners = true;

// weather or not thrusters are on when you start the script
public bool jetpack = false;

// weather or not cruise mode is on when you start the script
public bool cruise = false;

// make cruise mode act more like an airplane
public bool cruisePlane = false;

// this is used to identify blocks as belonging to this programmable block.
// pass the '%applyTags' argument, and the program will spread its tag across all blocks it controls.
// the program then won't touch any blocks that don't have the tag. unless you pass the '%removeTags' argument.
// if you add or remove a tag manually, pass the '%reset' argument to force a re-check
// if you make this tag unique to this ship, it won't interfere with your other vector thrust ships
public const string myName = "VT";
// normal: |VT|
public const string activeSurround = "|";
// standby: .VT.
public const string standbySurround = ".";

// put this in custom data of a cockpit to instruct the script to use a display in that cockpit
// it has to be on a line of its own, and have an integer after the ':'
// the integer must be 0 <= integer <= total # of displays in the cockpit
// eg:
//		%Vector:0
// this would make the script use the 1st display. the 1st display is #0, 2nd #1 etc..
// if you have trouble, look in the bottom right of the PB terminal, it will print errors there
public const string textSurfaceKeyword = "%Vector:";

// standby stops all calculations and safely turns off all nacelles, good if you want to stop flying
// but dont want to turn the craft off.
public const bool startInStandby = true;
// change this is you don't want the script to start in standby... please only use this if you have permission from the server owner

// set to -1 for the fastest speed in the game (changes with mods)
public float maxRotorRPM = 60f;

public const float defaultAccel = 1f;//this is the default target acceleration you see on the display
// if you want to change the default, change this
// note, values higher than 1 will mean your nacelles will face the ground when you want to go
// down rather than just lower thrust
// '1g' is acceleration caused by current gravity (not nessicarily 9.81m/s) although
// if current gravity is less than 0.1m/s it will ignore this setting and be 9.81m/s anyway

public const float accelBase = 1.5f;//accel = defaultAccel * g * base^exponent
// your +, - and 0 keys increment, decrement and reset the exponent respectively
// this means increasing the base will increase the amount your + and - change target acceleration

// multiplier for dampeners, higher is stronger dampeners
public const float dampenersModifier = 0.1f;


// default acceleration in situations with 0 (or low) gravity
public const float zeroGAcceleration = 9.81f;
// if gravity becomes less than this, zeroGAcceleration will kick in
public const float gravCutoff = 0.1f * zeroGAcceleration;


// determines when the thrusters will cut out in space.
public const float lowThrustCutOff = 0.1f;
// determines when the thrusters will turn back on. this should always be greater than cutoff
public const float lowThrustCutOn = 1.0f;

// true: only main cockpit can be used even if there is no one in the main cockpit
// false: any cockpits can be used, but if there is someone in the main cockpit, it will only obey the main cockpit
// no main cockpit: any cockpits can be used
public const bool onlyMainCockpit = true;

// Choose wether you want the script to update once every frame or once every 10 frames
// should be 1 of:
// UpdateFrequency.Update1
// UpdateFrequency.Update10
public const UpdateFrequency update_frequency = UpdateFrequency.Update1;

public const string LCDName = "%VectorLCD";

// arguments, you can change these to change what text you run the programmable block with
public const string standbytogArg = "%standby";
public const string standbyonArg = "%standbyenter";
public const string standbyoffArg = "%standbyexit";
public const string dampenersArg = "%dampeners";
public const string cruiseArg = "%cruise";
public const string jetpackArg = "%jetpack";
public const string raiseAccelArg = "%raiseAccel";
public const string lowerAccelArg = "%lowerAccel";
public const string resetAccelArg = "%resetAccel";
public const string resetArg = "%reset";//this one re-runs the initial setup... you probably want to use %resetAccel
public const string applyTagsArg = "%applyTags";
public const string removeTagsArg = "%removeTags";

public string[] allArgs = {
	standbytogArg,
	standbyonArg,
	standbyoffArg,
	dampenersArg,
	cruiseArg,
	jetpackArg,
	raiseAccelArg,
	lowerAccelArg,
	resetAccelArg,
	resetArg,
	applyTagsArg,
	removeTagsArg
};

// control module gamepad bindings
// type "/cm showinputs" into chat
// press the desired button
// put that text EXACTLY as it is in the quotes for the control you want
public const string jetpackButton = "c.thrusts";
public const string dampenersButton = "c.damping";
public const string cruiseButton = "c.cubesizemode";
public const string lowerAccel = "c.switchleft";
public const string raiseAccel = "c.switchright";
public const string resetAccel = "pipe";

// boost settings (this only works with control module)
// you can use this to set target acceleration values that you can quickly jump to by holding down the specified button
// there are defaults here:
// 	c.sprint (shift)	3g
// 	ctrl 			0.3g
public const bool useBoosts = true;

public KeyValuePair<string,float>[] boosts = {
	new KeyValuePair<string,float>("c.sprint", 3f),
	new KeyValuePair<string,float>("ctrl", 0.3f)
};



// DEPRECATED: use the tags instead
// only use blocks that have 'show in terminal' set to true
public bool ignoreHiddenBlocks = false;
// this can't be const, because it causes unreachable code warning, which now can't be disabled







//                              V 180 degrees
//              V 0 degrees                      V 360 degrees
// 				|-----\                    /------
// desired power|----------------------------------------- value of 0.1
// 				|       \                /
// 				|        \              /
// 				|         \            /
// no power 	|-----------------------------------------
//
//
// 				|-----\                    /------stuff above desired power gets set to desired power
// 				|      \                  /
// 				|       \                /
// desired power|----------------------------------------- value of 0.8
// 				|         \            /
// no power 	|-----------------------------------------

// the above pictures are for 'thrustModifierAbove', the same principle applies for 'thrustModifierBelow', except it goes below the 0 line, instead of above the max power line.
// the clipping value 'thrustModifier' defines how far the thruster can be away from the desired direction of thrust, and have the power still at desired power, otherwise it will be less
// these values can only be between 0 and 1

// another way to look at it is:
// set above to 1, its at 100% of desired power far from the direction of thrust
// set below to 1, its at 000% of desired power far from the opposite direction of thrust

// set above to 0, its at 100% of desired power only when it is exactly in the direction of thrust
// set below to 0, its at 000% of desired power only when it is exactly in the opposite direction of thrust

public const double thrustModifierAboveSpace = 0.01;
public const double thrustModifierBelowSpace = 0.9;

public const double thrustModifierAboveGrav = 0.1;
public const double thrustModifierBelowGrav = 0.1;

























































// use control module... this can always be true
public bool controlModule = true;

public Program() {
	Echo("Just Compiled");
	programCounter = 0;
	gotNacellesCount = 0;
	updateNacellesCount = 0;
	Runtime.UpdateFrequency = UpdateFrequency.Once;
	this.greedy = !hasTag(Me);
	if(Me.CustomData.Equals("")) {
		Me.CustomData = textSurfaceKeyword + 0;
	}
}

public void Save() {}

//at 60 fps this will last for 9000+ hrs before going negative
public long programCounter;
public long gotNacellesCount;
public long updateNacellesCount;

string lastArg = "";

// THE Issue: Any thruster(s) on a rotor when thrusting apply a torque that turns the thruster to face the center of gravity of the ship.
// The more thrust, the stronger this force, and it can exceed the maximum torque that can be set on the rotor, at which point the nacelle
// can no longer be controlled at all (before that, the nacelle might already see some adverse affects, turning slower/faster than intended).
// The further away a thruster is from the center of mass (ignoring the axis of rotation of the rotor), the stronger the effect.
// That is why building aligned with the center of mass helps, but since cargo affects the center of mass, this doesn't work well for cargo haulers.
public void Main(string argument, UpdateType runType) {

	if(argument.Length != 0) {
		lastArg = argument;
	}

	// Only accept arguments on certain update types
	UpdateType valid_argument_updates = UpdateType.None;
	valid_argument_updates |= UpdateType.Terminal;
	valid_argument_updates |= UpdateType.Trigger;
	valid_argument_updates |= UpdateType.Script;
	if((runType & valid_argument_updates) == UpdateType.None) {
		argument = "";
	}

	if(!doStartup(argument, runType)) {
		return;
	}

	MatrixD shipWorldMatrix = mainController != null ? mainController.WorldMatrix : usableControllers[0].WorldMatrix;
	MatrixD worldShipMatrix = MatrixD.Invert(shipWorldMatrix);

	// Get a vector indicating where we want to go, in world space.
	// For each key held a vector length 1 is added in the respective direction, analog controllers are analog and might have a bigger value.
	Vector3D desiredVec = getMovementInput(argument);


	Vector3D shipVel;
	float shipPhysicalMass;
	float worldGravAccMag;
	Vector3D requiredThrustVec;
 
	doPhysics(desiredVec, out shipVel, out shipPhysicalMass, out worldGravAccMag, out requiredThrustVec);



	// ========== DISTRIBUTE THE FORCE EVENLY BETWEEN NACELLES ==========

	// hysteresis
	if(requiredThrustVec.Length() > lowThrustCutOn * gravCutoff * shipPhysicalMass) {//TODO: this causes problems if there are many small nacelles
		thrustOn = true;
	}
	if(requiredThrustVec.Length() < lowThrustCutOff * gravCutoff * shipPhysicalMass) {
		thrustOn = false;
	}

	//Echo($"thrustOn: {thrustOn} \n{Math.Round(requiredThrustVec.Length()/(gravCutoff*shipPhysicalMass), 2)}\n{Math.Round(requiredThrustVec.Length()/(gravCutoff*shipPhysicalMass*0.01), 2)}");

	// maybe lerp this in the future
	if(!thrustOn) {// Zero G
		//Echo("\nnot much thrust");
		Vector3D zero_G_accel = (shipWorldMatrix.Down + shipWorldMatrix.Backward) * zeroGAcceleration / 1.414f;
		if(dampeners) {
			requiredThrustVec = zero_G_accel * shipPhysicalMass + requiredThrustVec;
		} else {
			requiredThrustVec = (requiredThrustVec - shipVel) + zero_G_accel;
		}
	}

	// update thrusters on/off and re-check nacelles direction
	// When we switch to/from fake zero-G grav I guess, which is usually near where atmospheric thrusters stop working?
	bool gravChanged = Math.Abs(lastGrav - worldGravAccMag) > 0.05f;
	lastGrav = worldGravAccMag;
	bool reGroupNacelles = false;
	foreach(Nacelle n in nacelles) {
		// we want to update if the thrusters are not valid, or atmosphere has changed
		if(!n.validateThrusters(jetpack) || gravChanged) {
			n.detectThrustDirection();
			reGroupNacelles = true;
		}

		// These are set by doPhysics
		n.thrustModifierAbove = thrustModifierAbove;
		n.thrustModifierBelow = thrustModifierBelow;

		// This needs to be done all the time, because atmospheric thrusters vary with atmosphere density, which changes with height.
		n.calcMaxEffectiveThrust();
	}

	// We expect any rotor to be aligned with one of the ships primary axes.
	// Otherwise solving becomes far harder (and it's hard enough already!).
	// So stick rotors on rotors or hinges, and you get what you deserve.

	// We might want to do this more often if we want to support nacelles with mixed thruster types in different directions.
	if(reGroupNacelles || (nacelleGroups.Count == 0 && nacelles.Count != 0)) {
		// Create/reset the groups
		foreach (Base6Directions.Axis axis in Enum.GetValues(typeof(Base6Directions.Axis))) {
			nacelleGroups[axis] = new List<Nacelle>();
		}		

		// Group nacelles among the 3 primary axes.
		foreach(Nacelle nacelle in nacelles) {
			Vector3D rotorUpInShipSpace = Vector3D.TransformNormal(nacelle.rotor.WorldMatrix.Up, worldShipMatrix);
			Base6Directions.Axis axis = Base6Directions.GetAxis(Base6Directions.GetDirection(rotorUpInShipSpace));
			nacelleGroups[axis].Add(nacelle);
		}
	}

	// The main solver, note that 'regular' thrusters are controlled by the game itself,
	// and their thrust has already been subtracted from the required thrust vector.
	// So first the game tries to achieve wanted dampening + acceleration with the regular thrusters.
	// And then we do the same, but on top of the solution the game has already set.
	// Note that the game does not seem to dampen against thrusters set to a value,
	// which is good since it won't fight us, but it also won't compensate any mistakes we make.
	// Also note that if the game misses (counter) thrust in one direction, it will not throttle down other
	// directions to prevent drifting that way. This is also good, since we can then provide the needed counter thrust.

	// Get the group sizes for scaling, because of phantom torque we want force spread evenly over each nacelle(rotor).
	float forwardBackwardSize = nacelleGroups[Base6Directions.Axis.ForwardBackward].Count;
	float leftRightSize = nacelleGroups[Base6Directions.Axis.LeftRight].Count;
	float upDownSize = nacelleGroups[Base6Directions.Axis.UpDown].Count;

	// The ? : here are to prevent division by zero if two groups are empty.
	float xScaleUp = upDownSize == 0 ? 0 : upDownSize / (upDownSize + forwardBackwardSize);
	float yScaleRight = leftRightSize == 0 ? 0 : leftRightSize / (leftRightSize + forwardBackwardSize);
	float zScaleRight = leftRightSize == 0 ? 0 : leftRightSize / (leftRightSize + upDownSize);

	Vector3D reqThrustShipSpace = Vector3D.TransformNormal(requiredThrustVec, worldShipMatrix);

	// Apply first nacelle settings to rest in each group, split between the nacelles in the group based on available thrust.
	foreach(Base6Directions.Axis axis in nacelleGroups.Keys) {
		// write($"-- Group: {axis}");
		List<Nacelle> group = nacelleGroups[axis];
		if(group.Count == 0) {
			continue;
		}
		Vector3D groupThrustVec;
		if(axis == Base6Directions.Axis.ForwardBackward) {
			groupThrustVec = new Vector3D(reqThrustShipSpace.X * (1.0 - xScaleUp), reqThrustShipSpace.Y * (1.0 - yScaleRight), 0);
		} else if( axis == Base6Directions.Axis.LeftRight) {
			groupThrustVec =  new Vector3D(0, reqThrustShipSpace.Y * yScaleRight, reqThrustShipSpace.Z * zScaleRight);
		} else {
			groupThrustVec = new Vector3D(reqThrustShipSpace.X * xScaleUp, 0, reqThrustShipSpace.Z * (1.0 - zScaleRight));
		}
		Vector3D reqFull = Vector3D.TransformNormal(groupThrustVec, shipWorldMatrix);
		float groupMaxEffectiveThrust = group.Sum(n => n.maxEffectiveThrust);
		foreach(Nacelle n in group) {
			n.requiredThrustVec = (n.maxEffectiveThrust / groupMaxEffectiveThrust) * reqFull;
			n.go(jetpack);
			// n.diag();
		}
	}


	write($"Last cmd: {lastArg}");
	write($"Target Accel: {getAccelerationMul():N2}g");
	write($"Thrusters: {jetpack}");
	write($"Dampeners: {dampeners}");
	write($"Cruise: {cruise}");
	write($"Active Nacelles: {nacelles.Count}");//TODO: make activeNacelles account for the number of nacelles that are actually active (activeThrusters.Count > 0)
	// write("Got Nacelles: " + gotNacellesCount);
	// write("Update Nacelles: " + updateNacellesCount);
	// ========== END OF MAIN ==========



	// echo the errors with surface provider
	Echo(surfaceProviderErrorStr);
}


string surfaceProviderErrorStr = "";

int accelExponent = 0;

bool jetpackIsPressed = false;
bool dampenersIsPressed = false;
bool cruiseIsPressed = false;
bool plusIsPressed = false;
bool minusIsPressed = false;

bool globalAppend = false;

IMyShipController mainController = null;
List<IMyShipController> allControllers = new List<IMyShipController>();
List<IMyShipController> usableControllers = new List<IMyShipController>();
List<Nacelle> nacelles = new List<Nacelle>();
Dictionary<Base6Directions.Axis, List<Nacelle>> nacelleGroups = new Dictionary<Base6Directions.Axis, List<Nacelle>>();

List<IMyThrust> normalThrusters = new List<IMyThrust>();
List<IMyTextPanel> allScreens = new List<IMyTextPanel>();
List<IMyTextPanel> usableScreens = new List<IMyTextPanel>();
HashSet<IMyTextSurface> surfaces = new HashSet<IMyTextSurface>();

float oldMass = 0;

int rotorCount = 0;
int rotorTopCount = 0;
int thrusterCount = 0;

bool standby = startInStandby;
double thrustModifierAbove = 0.1;// how close the rotor has to be to target position before the thruster gets to full power
double thrustModifierBelow = 0.1;// how close the rotor has to be to opposite of target position before the thruster gets to 0 power

bool justCompiled = true;
bool goToStandby = false;
bool comeFromStandby = false;

const string tag = activeSurround + myName + activeSurround;
const string offtag = standbySurround + myName + standbySurround;

bool applyTags = false;
bool removeTags = false;
bool greedy = true;
float lastGrav = -1;
public bool thrustOn = false;

Dictionary<string, object> CMinputs = null;

bool doStartup(string argument, UpdateType runType) {
	globalAppend = false;

	programCounter++;
	Echo($"Last Runtime {Runtime.LastRunTimeMs:N2}ms #{programCounter}");
	write($"{"|\\-/"[(int)programCounter/10%4]} {Runtime.LastRunTimeMs:N0}ms");

	Echo($"Greedy: {greedy}");

	bool anyArg = Array.Exists(allArgs, arg => isArg(argument, arg));

	if(isArg(argument, standbyonArg) || 
	   (isArg(argument, standbytogArg) && !standby) ||
	   goToStandby) {
		enterStandby();
		return false;
	} else if(isArg(argument, standbyoffArg) ||
	          ((anyArg || runType == UpdateType.Terminal) && standby) ||
	          comeFromStandby) {
		exitStandby();
	} else {
		Echo("Normal Running");
	}

	if(justCompiled || allControllers.Count == 0 || isArg(argument, resetArg)) {
		Echo("Initialising..");
		getNacelles(true);
		List<IMyShipController> conts = new List<IMyShipController>();
		GridTerminalSystem.GetBlocksOfType<IMyShipController>(conts);
		if(!getControllers(conts)) {
			Echo("Init failed.");
			return false;
		}
		Echo("Init success.");
	}

	//tags and getting blocks
	applyTags = isArg(argument, applyTagsArg);
	removeTags = !applyTags && isArg(argument, removeTagsArg);
	// switch on: removeTags
	// switch off: applyTags
	greedy = (!applyTags && greedy) || removeTags;
	if(applyTags) {
		addTag(Me);
	} else if(removeTags) {
		removeTag(Me);
	}
	// this automatically calls getNacelles() as needed, and passes in previous GridTerminalSystem data
	if(!checkNacelles()) {
		Echo("Setup failed, stopping.");
		return false;
	}
	applyTags = false;
	removeTags = false;




	if(justCompiled) {
		justCompiled = false;
		Runtime.UpdateFrequency = UpdateFrequency.Once;
		if(Storage == "" || !startInStandby) {
			Storage = "Don't Start Automatically";
			// run normally
			comeFromStandby = true;
			return false;
		} else {
			// go into standby mode
			goToStandby = true;
			return false;
		}
	}

	if(standby) {
		Echo("Standing By");
		write("Standing By");
		return false;
	}

	return true;
}

void doPhysics(Vector3D desiredVec, out Vector3D shipVel, out float shipPhysicalMass, out float worldGravAccMag, out Vector3D requiredThrustVec) {

 	// Get gravity in world space, gravity is measured as acceleration in m/s^2.
	Vector3D worldGravAcc = usableControllers[0].GetNaturalGravity();

	// Get velocity, which is measured in m/s, just like you see in the hud.
	shipVel = usableControllers[0].GetShipVelocities().LinearVelocity;

	// Setup mass
	MyShipMass myShipMass = usableControllers[0].CalculateShipMass();
	shipPhysicalMass = myShipMass.PhysicalMass;

	if(myShipMass.BaseMass < 0.001f) {
		Echo("Can't fly a Station");
		shipPhysicalMass = 0.001f;
	}

	// Get the magintude of the gravity vector, use a fake value if we're not in a gravity well.
	worldGravAccMag = (float)worldGravAcc.Length();
	if(worldGravAccMag < gravCutoff) {
		worldGravAccMag = zeroGAcceleration;
		thrustModifierAbove = thrustModifierAboveSpace;
		thrustModifierBelow = thrustModifierBelowSpace;
	} else {
		thrustModifierAbove = thrustModifierAboveGrav;
		thrustModifierBelow = thrustModifierBelowGrav;
	}

	Vector3D shipWeightForce = shipPhysicalMass * worldGravAcc;

	if(dampeners) {
		Vector3D dampVec = Vector3D.Zero;

		// I think this prevents dampening in the direction we actually want to go.
		if(desiredVec != Vector3D.Zero) {
			// Cancel movement opposite to desired movement direction
			if(desiredVec.dot(shipVel) < 0) {
				// If you want to go opposite to velocity
				dampVec += shipVel.project(desiredVec.normalized());
			}
			// Cancel sideways movement
			dampVec += shipVel.reject(desiredVec.normalized());
		} else {
			dampVec += shipVel;
		}


		if(cruise) {

			foreach(IMyShipController cont in usableControllers) {
				if(onlyMain() && cont != mainController) continue;
				if(!cont.IsUnderControl) continue;


				if(dampVec.dot(cont.WorldMatrix.Forward) > 0 || cruisePlane) { // only front, or front+back if cruisePlane is activated
					dampVec -= dampVec.project(cont.WorldMatrix.Forward);
				}

				if(cruisePlane) {
					// Remove the ship forward/backward component of the force our weight excerts, so that we don't compensate it.
					// This results in us moving forward with nose down, and backwards with nose up.
					shipWeightForce -= shipWeightForce.project(cont.WorldMatrix.Forward);
				}
			}
		}

		desiredVec -= dampVec * dampenersModifier;
	}

	desiredVec *= shipPhysicalMass * worldGravAccMag * getAccelerationMul();

	// point thrust in opposite direction, add weight. this is force, not acceleration
	requiredThrustVec = -desiredVec + shipWeightForce;

	// Remove thrust done by normal thrusters, note that thrust points to Forward, so adding Backward has the same effect as subtracting.
	foreach(IMyThrust normalThruster in normalThrusters) {
		requiredThrustVec += normalThruster.WorldMatrix.Backward * normalThruster.CurrentThrust;
	}

	Echo($"Required Force: {requiredThrustVec.Length():N0}N");
}

// Pretty print the given normal after conversion by the given matrix.
public static String formatNormal(ref Vector3D normal, ref MatrixD matrix) {
	Vector3D transformed;
	Vector3D.TransformNormal(ref normal, ref matrix, out transformed);
	return formatVec(ref transformed);
}

// Pretty print the given vector.
public static String formatVec(ref Vector3D vec) {
	return $"X:{vec.X,11:N2} Y:{vec.Y,11:N2} Z:{vec.Z,11:N2}";
}

bool isArg(string argument, string toCheck) {
	return argument.ToLower().Contains(toCheck.ToLower());
}

void enterStandby() {
	standby = true;
	goToStandby = false;

	//set status of blocks
	foreach(Nacelle n in nacelles) {
		n.rotor.Enabled = false;
		standbyTag(n.rotor);
		foreach(Thruster t in n.thrusters) {
			t.theBlock.Enabled = false;
			standbyTag(t.theBlock);
		}
	}
	foreach(IMyTextPanel screen in usableScreens) {
		standbyTag(screen);
	}
	foreach(IMyShipController cont in usableControllers) {
		standbyTag(cont);
	}
	standbyTag(Me);

	Runtime.UpdateFrequency = UpdateFrequency.None;

	Echo("Standing By");
	write("Standing By");
}

void exitStandby() {
	standby = false;
	comeFromStandby = false;

	//set status of blocks
	foreach(Nacelle n in nacelles) {
		n.rotor.Enabled = true;
		activeTag(n.rotor);
		foreach(Thruster t in n.thrusters) {
			if(t.IsOn) {
				t.theBlock.Enabled = true;
			}
			activeTag(t.theBlock);
		}
	}
	foreach(IMyTextPanel screen in usableScreens) {
		activeTag(screen);
	}
	foreach(IMyShipController cont in usableControllers) {
		activeTag(cont);
	}
	activeTag(Me);

	Runtime.UpdateFrequency = update_frequency;

	Echo("Resuming from standby");
}

bool hasTag(IMyTerminalBlock block) {
	return block.CustomName.Contains(tag) || block.CustomName.Contains(offtag);
}

void addTag(IMyTerminalBlock block) {
	string name = block.CustomName;

	if(name.Contains(tag)) {
		// there is already a tag, just set it to current status
		if(standby) {
			block.CustomName = name.Replace(tag, offtag);
		}

	} else if(name.Contains(offtag)) {
		// there is already a tag, just set it to current status
		if(!standby) {
			block.CustomName = name.Replace(offtag, tag);
		}

	} else {
		// no tag found, add tag to start of string

		if(standby) {
			block.CustomName = offtag + " " + name;
		} else {
			block.CustomName = tag + " " + name;
		}
	}

}

void removeTag(IMyTerminalBlock block) {
	block.CustomName = block.CustomName.Replace(tag, "").Trim();
	block.CustomName = block.CustomName.Replace(offtag, "").Trim();
}

void standbyTag(IMyTerminalBlock block) {
	block.CustomName = block.CustomName.Replace(tag, offtag);
}

void activeTag(IMyTerminalBlock block) {
	block.CustomName = block.CustomName.Replace(offtag, tag);
}


// true: only main cockpit can be used even if there is no one in the main cockpit
// false: any cockpits can be used, but if there is someone in the main cockpit, it will only obey the main cockpit
// no main cockpit: any cockpits can be used
bool onlyMain() {
	return mainController != null && (mainController.IsUnderControl || onlyMainCockpit);
}

void getScreens() {
	getScreens(allScreens);
}

void getScreens(List<IMyTextPanel> newScreens) {
	allScreens = newScreens;
	usableScreens.Clear();
	foreach(IMyTextPanel screen in allScreens) {
		if(removeTags) {
			removeTag(screen);
		}

		bool actGreedy = greedy || applyTags || removeTags;
		if ( (!actGreedy && !hasTag(screen)) ||
			(!screen.IsWorking) ||
			(!hasTag(screen) && !screen.CustomName.ToLower().Contains(LCDName.ToLower())) ) {
			surfaces.Remove(screen);
			continue;
		}
		if(applyTags) {
			addTag(screen);
		}
		usableScreens.Add(screen);
		surfaces.Add(screen);
	}
}

public void write(string str) {
	if(this.surfaces.Count > 0) {
		str += "\n";
		foreach(IMyTextSurface surface in this.surfaces) {
			surface.WriteText(str, globalAppend);
			surface.ContentType = ContentType.TEXT_AND_IMAGE;
		}
	} else if(!globalAppend) {
		Echo("No text surfaces available");
	}
	globalAppend = true;
}

float getAccelerationMul() {
	// Look through boosts, applies acceleration of first one found. If none found or boosts not enabled, go for normal accel
	int boostIndex = useBoosts && this.controlModule ? Array.FindIndex(this.boosts, boost => CMinputs.ContainsKey(boost.Key)) : -1;
	float accel = boostIndex != -1 ? this.boosts[boostIndex].Value : (float)Math.Pow(accelBase, accelExponent);
	return accel * defaultAccel;
}

Vector3D getMovementInput(string arg) {
	if(controlModule) {
		// setup control module
		Dictionary<string, object> inputs = new Dictionary<string, object>();
		try {
			CMinputs = Me.GetValue<Dictionary<string, object>>("ControlModule.Inputs");
			Me.SetValue<string>("ControlModule.AddInput", "all");
			Me.SetValue<bool>("ControlModule.RunOnInput", true);
			Me.SetValue<int>("ControlModule.InputState", 1);
			Me.SetValue<float>("ControlModule.RepeatDelay", 0.016f);
		} catch(Exception e) {
			controlModule = false;
		}
	}

	if(controlModule) {
		// non-movement controls
		if(CMinputs.ContainsKey(dampenersButton) && !dampenersIsPressed) {//inertia dampener key
			dampeners = !dampeners;//toggle
			dampenersIsPressed = true;
		}
		if(!CMinputs.ContainsKey(dampenersButton)) {
			dampenersIsPressed = false;
		}


		if(CMinputs.ContainsKey(cruiseButton) && !cruiseIsPressed) {//cruise key
			cruise = !cruise;//toggle
			cruiseIsPressed = true;
		}
		if(!CMinputs.ContainsKey(cruiseButton)) {
			cruiseIsPressed = false;
		}

		if(CMinputs.ContainsKey(jetpackButton) && !jetpackIsPressed) {//jetpack key
			jetpack = !jetpack;//toggle
			jetpackIsPressed = true;
		}
		if(!CMinputs.ContainsKey(jetpackButton)) {
			jetpackIsPressed = false;
		}

		if(CMinputs.ContainsKey(raiseAccel) && !plusIsPressed) {//throttle up
			accelExponent++;
			plusIsPressed = true;
		}
		if(!CMinputs.ContainsKey(raiseAccel)) { //increase target acceleration
			plusIsPressed = false;
		}

		if(CMinputs.ContainsKey(lowerAccel) && !minusIsPressed) {//throttle down
			accelExponent--;
			minusIsPressed = true;
		}
		if(!CMinputs.ContainsKey(lowerAccel)) { //lower target acceleration
			minusIsPressed = false;
		}

		if(CMinputs.ContainsKey(resetAccel)) { //default target acceleration
			accelExponent = 0;
		}

	}

	bool changeDampeners = false;
	if(isArg(arg, dampenersArg)) {
		dampeners = !dampeners;
		changeDampeners	= true;
	}
	if(isArg(arg, cruiseArg)) {
		cruise = !cruise;
	}
	if(isArg(arg, jetpackArg)) {
		jetpack = !jetpack;
	}
	if(isArg(arg, raiseAccelArg)) {
		accelExponent++;
	}
	if(isArg(arg, lowerAccelArg)) {
		accelExponent--;
	}
	if(isArg(arg, resetAccelArg)) {
		accelExponent = 0;
	}

	// dampeners (if there are any normal thrusters, the dampeners control works)
	if(normalThrusters.Count != 0) {

		if(onlyMain()) {

			if(changeDampeners) {
				mainController.DampenersOverride = dampeners;
			} else {
				dampeners = mainController.DampenersOverride;
			}
		} else {

			if(changeDampeners) {
				// make all conform
				foreach(IMyShipController cont in usableControllers) {
					cont.DampenersOverride = dampeners;
				}
			} else {

				// check if any are different to us
				bool any_different = false;
				foreach(IMyShipController cont in usableControllers) {
					if(cont.DampenersOverride != dampeners) {
						any_different = true;
						dampeners = cont.DampenersOverride;
						break;
					}
				}

				if(any_different) {
					// update all others to new value too
					foreach(IMyShipController cont in usableControllers) {
						cont.DampenersOverride = dampeners;
					}
				}
			}
		}
	}

	// Movement controls
	Vector3D moveVec;
	if(onlyMain()) {
		moveVec = mainController.getWorldMoveIndicator();
	} else {
		moveVec = Vector3D.Zero;
		foreach(IMyShipController cont in usableControllers) {
			if(cont.IsUnderControl) {
				moveVec += cont.getWorldMoveIndicator();
			}
		}
	}

	return moveVec;
}

void removeSurface(IMyTextSurface surface) {
	if(surfaces.Contains(surface)) {
		//need to check this, because otherwise it will reset panels
		//we aren't controlling
		surfaces.Remove(surface);
		surface.ContentType = ContentType.NONE;
		surface.WriteText("", false);
	}
}

bool removeSurfaceProvider(IMyTerminalBlock block) {
	if(!(block is IMyTextSurfaceProvider)) return false;
	IMyTextSurfaceProvider provider = (IMyTextSurfaceProvider)block;

	for(int i = 0; i < provider.SurfaceCount; i++) {
		if(surfaces.Contains(provider.GetSurface(i))) {
			removeSurface(provider.GetSurface(i));
		}
	}
	return true;
}
bool addSurfaceProvider(IMyTerminalBlock block) {
	if(!(block is IMyTextSurfaceProvider)) return false;
	IMyTextSurfaceProvider provider = (IMyTextSurfaceProvider)block;
	bool retval = true;

	if(block.CustomData.Length == 0) {
		return false;
	}

	bool [] to_add = new bool[provider.SurfaceCount];
	for(int i = 0; i < to_add.Length; i++) {
		to_add[i] = false;
	}

	int begin_search = 0;
	while(begin_search >= 0) {
		string data = block.CustomData;
		int start = data.IndexOf(textSurfaceKeyword, begin_search);

		if(start < 0) {
			// true if it found at least 1
			retval =  begin_search != 0;
			break;
		}
		int end = data.IndexOf("\n", start);
		begin_search = end;

		string display = "";
		if(end < 0) {
			display = data.Substring(start + textSurfaceKeyword.Length);
		} else {
			display = data.Substring(start + textSurfaceKeyword.Length, end - (start + textSurfaceKeyword.Length) );
		}

		int display_num = 0;
		if(Int32.TryParse(display, out display_num)) {
			if(display_num >= 0 && display_num < provider.SurfaceCount) {
				// it worked, add the surface
				to_add[display_num] = true;

			} else {
				// range check failed
				string err_str = "";
				if(end < 0) {
					err_str = data.Substring(start);
				} else {
					err_str = data.Substring(start, end - (start) );
				}
				surfaceProviderErrorStr += $"\nDisplay number out of range: {display_num}\nshould be: 0 <= num < {provider.SurfaceCount}\non line: ({err_str})\nin block: {block.CustomName}\n";
			}

		} else {
			//didn't parse
			string err_str = "";
			if(end < 0) {
				err_str = data.Substring(start);
			} else {
				err_str = data.Substring(start, end - (start) );
			}
			surfaceProviderErrorStr += $"\nDisplay number invalid: {display}\non line: ({err_str})\nin block: {block.CustomName}\n";
		}
	}

	for(int i = 0; i < to_add.Length; i++) {
		if(to_add[i]) {
			surfaces.Add(provider.GetSurface(i));
		} else {
			removeSurface(provider.GetSurface(i));
		}
	}


	return retval;
}

bool getControllers() {
	return getControllers(allControllers);
}

bool getControllers(List<IMyShipController> blocks) {
	mainController = null;
	allControllers = blocks;

	usableControllers.Clear();

	string reason = "";
	bool actGreedy = greedy || applyTags || removeTags;
	foreach(IMyShipController block in blocks) {
		bool canAdd = true;
		string currreason = block.CustomName + "\n";
		if(!block.ShowInTerminal && ignoreHiddenBlocks) {
			currreason += "  ShowInTerminal not set\n";
			canAdd = false;
		}
		if(!block.CanControlShip) {
			currreason += "  CanControlShip not set\n";
			canAdd = false;
		}
		if(!block.ControlThrusters) {
			currreason += "  can't ControlThrusters\n";
			canAdd = false;
		}
		if(block.IsMainCockpit) {
			mainController = block;
		}
		if(!(actGreedy || hasTag(block))) {
			currreason += "  Doesn't match my tag\n";
			canAdd = false;
		}
		if(removeTags) {
			removeTag(block);
		}

		if(canAdd) {
			addSurfaceProvider(block);
			usableControllers.Add(block);
			if(applyTags) {
				addTag(block);
			}
		} else {
			removeSurfaceProvider(block);
			reason += currreason;
		}
	}
	if(blocks.Count == 0) {
		reason += "no controllers\n";
	}

	if(usableControllers.Count == 0) {
		Echo("ERROR: no usable ship controller found. Reason: \n");
		Echo(reason);
		return false;
	}

	return true;
}

IMyShipController findACockpit() {
	foreach(IMyShipController cont in usableControllers) {
		if(cont.IsWorking) {
			return cont;
		}
	}

	return null;
}

// checks to see if the nacelles have changed
bool checkNacelles() {
	Echo("Checking Nacelles..");

	bool tagCommand = applyTags || removeTags;

	IMyShipController cont = findACockpit();
	if(cont == null) {
		Echo("No cockpit registered, checking everything.");
	} else if(!tagCommand) {
		float shipBaseMass = cont.CalculateShipMass().BaseMass;
		if(oldMass == shipBaseMass) {
			Echo("Mass is the same, everything is good.");

			// they may have changed the screen name to be a VT one
			getControllers();
			getScreens();
			return true;
		}
		Echo("Mass is different, checking everything.");
		oldMass = shipBaseMass;
		// surface may be exploded if mass changes, in this case, ghost surfaces my be left behind
		surfaces.Clear();
	}

	var conts = new List<IMyShipController>();
	var txts = new List<IMyTextPanel>();

	var blocks = new List<IMyTerminalBlock>();
	GridTerminalSystem.GetBlocksOfType<IMyTerminalBlock>(blocks, block => block is IMyShipController || block is IMyTextPanel);
	foreach(IMyTerminalBlock block in blocks) {
		if(block is IMyShipController) {
			conts.Add((IMyShipController)block);
		}
		if(block is IMyTextPanel) {
			txts.Add((IMyTextPanel)block);
		}
	}

	if(Me.SurfaceCount > 0) {
		surfaceProviderErrorStr = "";
		Me.CustomData = textSurfaceKeyword + 0;
		addSurfaceProvider(Me);
		Me.GetSurface(0).FontSize = 2.2f;// this isn't really the right place to put this, but doing it right would be a lot more code
	}

	// if you use the following if statement, it won't lock the non-main cockpit if someone sets the main cockpit, until a recompile or world load :/
	if(/*(mainController != null ? !mainController.IsMainCockpit : false) || */allControllers.Count != conts.Count || cont == null || tagCommand) {
		Echo($"Controller count ({allControllers.Count}) is out of whack (current: {conts.Count})");
		if(!getControllers(conts)) {
			return false;
		}
	}

	if(allScreens.Count != txts.Count || tagCommand) {
		Echo($"Screen count ({allScreens.Count}) is out of whack (current: {txts.Count})");
		getScreens(txts);
	} else {
		//probably may-aswell just getScreens either way. seems like there wouldn't be much performance hit
		getScreens(txts);
	}

	getNacelles(false);

	return true;
}

void getNacelles(bool initialize) {

	bool tagCommand = applyTags || removeTags;

	var rotors = new List<IMyMotorStator>();
	var thrusters = new List<IMyThrust>();

	var blocks = new List<IMyTerminalBlock>();
	GridTerminalSystem.GetBlocksOfType<IMyTerminalBlock>(blocks, block => block is IMyMotorStator || block is IMyThrust);
	foreach(IMyTerminalBlock block in blocks) {
		if(block is IMyThrust) {
			thrusters.Add((IMyThrust)block);
		} else {
			rotors.Add((IMyMotorStator)block);
		}
	}
	rotorCount = rotors.Count;
	thrusterCount = thrusters.Count;

	if(!initialize) {
		bool updateNacelles = false;

		if(rotorCount != rotors.Count) {
			Echo($"Rotor count ({rotorCount}) is out of whack (current: {rotors.Count})");
			updateNacelles = true;
		}

		var rotorHeads = new List<IMyAttachableTopBlock>();
		foreach(IMyMotorStator rotor in rotors) {
			if(rotor.Top != null) {
				rotorHeads.Add(rotor.Top);
			}
		}
		if(rotorTopCount != rotorHeads.Count) {
			Echo($"Rotor Head count ({rotorTopCount}) is out of whack (current: {rotorHeads.Count})");
			Echo($"Rotors: {rotors.Count}");
			updateNacelles = true;
		}

		if(thrusterCount != thrusters.Count) {
			Echo($"Thruster count ({thrusterCount}) is out of whack (current: {thrusters.Count})");
			updateNacelles = true;
		}

		if(updateNacelles || tagCommand) {
			Echo("Updating Nacelles");
		} else {
			Echo("They seem fine.");
			return;
		}
	}


	bool actGreedy = greedy || applyTags || removeTags;
	gotNacellesCount++;
	nacelles.Clear();

	Echo("Getting Rotors");
	// make this.nacelles out of all valid rotors
	rotorTopCount = 0;
	foreach(IMyMotorStator current in rotors) {
		if(removeTags) {
			removeTag(current);
		} else if(applyTags) {
			addTag(current);
		}


		if(!(actGreedy || hasTag(current))) { continue; }

		if(current.Top == null) {
			continue;
		} else {
			rotorTopCount++;
		}

		//if topgrid is not programmable blocks grid
		if(current.TopGrid == Me.CubeGrid) {
			continue;
		}

		// it's not set to not be a nacelle rotor
		// it's topgrid is not the programmable blocks grid
		nacelles.Add(new Nacelle(current, maxRotorRPM, this));
	}

	Echo("Getting Thrusters");
	// add all thrusters to their corresponding nacelle and remove nacelles that have none
	for(int i = nacelles.Count-1; i >= 0; i--) {
		for(int j = thrusters.Count-1; j >= 0; j--) {
			if(!(actGreedy || hasTag(thrusters[j]))) { continue; }
			if(removeTags) {
				removeTag(thrusters[j]);
			}

			if(thrusters[j].CubeGrid != nacelles[i].rotor.TopGrid) continue;// thruster is not for the current nacelle
			// if(!thrusters[j].IsFunctional) continue;// broken, don't add it

			if(applyTags) {
				addTag(thrusters[j]);
			}

			nacelles[i].thrusters.Add(new Thruster(thrusters[j]));

			// Shorten the list we have to check, and remove from the list of normal thrusters:
			thrusters.RemoveAt(j);
		}
		if(nacelles[i].thrusters.Count == 0) {
			// remove nacelles (rotors) without thrusters
			removeTag(nacelles[i].rotor);
			nacelles.RemoveAt(i);// there is no more reference to the rotor, should be garbage collected
		} else {
			// if its still there, setup the nacelle
			nacelles[i].validateThrusters(jetpack);
			nacelles[i].detectThrustDirection();
		}
	}
	normalThrusters = thrusters;
}

public class Nacelle {
	String errStr;
	Program program;

	// physical parts
	public IMyMotorStator rotor;
	// Thrust direction in rotor top local space.
	Vector3D direction = Vector3D.Zero;
	// Max RPM allowed for rotor.
	float maxRPM;


	public HashSet<Thruster> thrusters;// all the thrusters
	HashSet<Thruster> availableThrusters;// <= thrusters: the ones the user chooses to be used (ShowInTerminal)
	HashSet<Thruster> activeThrusters;// <= activeThrusters: the ones that are facing the direction that produces the most thrust (only recalculated if available thrusters changes)

	public double thrustModifierAbove = 0.1;// how close the rotor has to be to target position before the thruster gets to full power
	public double thrustModifierBelow = 0.1;// how close the rotor has to be to opposite of target position before the thruster gets to 0 power

	bool oldJetpack = true;
	public Vector3D requiredThrustVec = Vector3D.Zero;

	public float maxEffectiveThrust = 0.0f;
	int detectThrustCounter = 0;

	public Nacelle(IMyMotorStator rotor, float maxRotorRPM, Program program) {
		// don't want IMyMotorBase, that includes wheels

		this.program = program;
		this.rotor = rotor;
		if(maxRotorRPM <= 0) {
			maxRPM = rotor.GetMaximum<float>("Velocity");
		} else {
			maxRPM = maxRotorRPM;
		}
		thrusters = new HashSet<Thruster>();
		availableThrusters = new HashSet<Thruster>();
		activeThrusters = new HashSet<Thruster>();
		errStr = "";
	}

	// final calculations and setting physical components
	public void go(bool jetpack) {
		errStr = "";

		// Adjust the rotor angle by setting it's speed, returns the current angle.
		double angleCos = updateRotorVelocity();
		updateThrustersThrust(jetpack, angleCos);

		oldJetpack = jetpack;
	}

	public void diag() {
		program.write($"- Nacelle on {rotor.CustomName}");
		program.write($"  Thrst: {activeThrusters.Count}/{availableThrusters.Count}/{thrusters.Count} upd: {detectThrustCounter}");
		program.write($"  Required force: {requiredThrustVec.Length():N2}N / {maxEffectiveThrust:N2}N");
		program.write(errStr);
	}

	public void calcMaxEffectiveThrust() {
		maxEffectiveThrust = activeThrusters.Sum(t => t.theBlock.MaxEffectiveThrust);
	}

	void updateThrustersThrust(bool jetpack, double angleCos) {
		// the clipping value 'thrustModifier' defines how far the rotor can be away from the desired direction of thrust, and have the power still at max
		// if 'thrustModifier' is at 1, the thruster will be at full desired power when it is at 90 degrees from the direction of travel
		// if 'thrustModifier' is at 0, the thruster will only be at full desired power when it is exactly at the direction of travel, (it's never exactly in-line)
		// double thrustOffset = (angleCos + 1) / (1 + (1 - Program.thrustModifierAbove));//put it in some graphing calculator software where 'angleCos' is cos(x) and adjust the thrustModifier value between 0 and 1, then you can visualise it
		double abo = MathHelperD.Clamp(thrustModifierAbove, 0, 1);
		double bel = MathHelperD.Clamp(thrustModifierBelow, 0, 1);
		// put it in some graphing calculator software where 'angleCos' is cos(x) and adjust the thrustModifier values between 0 and 1, then you can visualise it
		double thrustOffset = ((((angleCos + 1) * (1 + bel)) / 2) - bel) * (((angleCos + 1) * (1 + abo)) / 2);// the other one is simpler, but this one performs better
		// double thrustOffset = (angleCos * (1 + abo) * (1 + bel) + abo - bel + 1) / 2;
		thrustOffset = MathHelperD.Clamp(thrustOffset, 0, 1);
		// Apply the offset
		double offsetThrust = thrustOffset * requiredThrustVec.Length();

		//set the thrust for each engine
		// errStr += $"\n=======thrusters=======";
		foreach(Thruster thruster in activeThrusters) {
			// errStr += thrustOffset.progressBar();
			double thrust = offsetThrust * thruster.theBlock.MaxEffectiveThrust / maxEffectiveThrust;
			bool noThrust = thrust < 0.03f;
			if(!jetpack || !program.thrustOn || noThrust) {
				thruster.setThrust(0);
				thruster.theBlock.Enabled = false;
				thruster.IsOffBecauseDampeners = !program.thrustOn || noThrust;
				thruster.IsOffBecauseJetpack = !jetpack;
			} else {
				thruster.setThrust(thrust);
				thruster.theBlock.Enabled = true;
				thruster.IsOffBecauseDampeners = false;
				thruster.IsOffBecauseJetpack = false;
			}

			// errStr += $"\nthruster '{thruster.theBlock.CustomName}': {thruster.errStr}\n";
		}
		// errStr += $"\n-------thrusters-------";
	}

	// This sets the rotor to face the desired direction in worldspace
	// The current requiredThrustVec must be in-line with the rotors plane of rotation
	double updateRotorVelocity() {
		// Normalize the target direction vector.
		Vector3D desiredVec = Vector3D.Normalize(requiredThrustVec);

		// The thrust direction of the current nacelle/rotor, in world space.
		Vector3D currentDir = Vector3D.TransformNormal(direction, rotor.Top.WorldMatrix);

		// The rotation axis of the current nacelle/rotor, in world space.
		Vector3D normal = rotor.WorldMatrix.Up;

		// This clever bit of math gives us the correctly signed sine of the angle between the target and current direction vectors.
		// But only if all three vectors are normalized, and both target and current direction vectors are in the plane perpendicular to the normal vector.
		double errSin = Vector3D.Dot(Vector3D.Cross(desiredVec, currentDir), normal);
		double errRad = Math.Asin(errSin);

		// Downside here is that thrustsers turn slower with slower updates, but otherwise we get wobble.
		int ticks = update_frequency == UpdateFrequency.Update1 ? 1 : 10;

		// We want to go fast, but not so fast we'll overshoot at the next update.
		// The normal rate is 60 ticks/s.
		// We use TargetVelocityRad, which is rad's per second.
		// So, try to rotate the rad we want, in one update worth of seconds.
		float maxRadS = (maxRPM / 30.0f) * (float)Math.PI;
		double secondsPerTick = 1.0 / 60.0;
		double secondsPerUpdate = secondsPerTick * ticks;
		double radS = errRad / secondsPerUpdate;

		// Using exactly 1 update causes all the wobble, probably because of inertia, so give it a few more updates.
		radS /= 4;

		if (radS > maxRadS) {
			rotor.TargetVelocityRad = maxRadS;
		} else if (radS * -1 > maxRadS) {
			rotor.TargetVelocityRad = maxRadS * -1;
		} else {
			rotor.TargetVelocityRad = (float)radS;
		}

		// Gets non signed cos(angle between 2 vectors), no need to divide by lenghts, since both vectors are already length 1.
		return Vector3D.Dot(currentDir, desiredVec);
	}

	//true if all thrusters are good
	public bool validateThrusters(bool jetpack) {
		bool needsUpdate = false;
		errStr += "validating thrusters: (jetpack {jetpack})\n";
		foreach(Thruster curr in thrusters) {

			bool shownAndFunctional = (curr.theBlock.ShowInTerminal || !program.ignoreHiddenBlocks) && curr.theBlock.IsFunctional;
			if(availableThrusters.Contains(curr)) {//is available
				errStr += "in available thrusters\n";

				bool wasOnAndIsNowOff = curr.IsOn && !curr.theBlock.Enabled && !curr.IsOffBecauseJetpack && !curr.IsOffBecauseDampeners;

				if((!shownAndFunctional || wasOnAndIsNowOff) && (jetpack && oldJetpack)) {
					// if jetpack is on, the thruster has been turned off
					// if jetpack is off, the thruster should still be in the group

					curr.IsOn = false;
					//remove the thruster
					availableThrusters.Remove(curr);
					needsUpdate = true;
				}

			} else {//not available
				errStr += "not in available thrusters\n";
				if(program.ignoreHiddenBlocks) {
					errStr += $"ShowInTerminal {curr.theBlock.ShowInTerminal}\n";
				}
				errStr += $"IsWorking {curr.theBlock.IsWorking}\n";
				errStr += $"IsFunctional {curr.theBlock.IsFunctional}\n";

				bool wasOffAndIsNowOn = !curr.IsOn && curr.theBlock.Enabled;
				if(shownAndFunctional && wasOffAndIsNowOn) {
					availableThrusters.Add(curr);
					needsUpdate = true;
					curr.IsOn = true;
				}
			}
		}
		return !needsUpdate;
	}

	public void detectThrustDirection() {
		detectThrustCounter++;
		Dictionary<Base6Directions.Direction, float> thrustPerDirection = new Dictionary<Base6Directions.Direction, float>();

		// Get the Up direction of the attached rotor top, in it's local grid space.
		Base6Directions.Direction rotTopUp = rotor.Top.Orientation.Up;

		// add all the thrusters effective power
		foreach(Thruster thruster in availableThrusters) {
			// Get the forward direction (which is where the exhaust points) of the thruster, in it's local grid space.
			Base6Directions.Direction thrustForward = thruster.theBlock.Orientation.Forward;

			// If its not facing rotor up or rotor down	
			if(Base6Directions.GetAxis(rotTopUp) != Base6Directions.GetAxis(thrustForward)) {
				if(!thrustPerDirection.ContainsKey(thrustForward)) {
					thrustPerDirection[thrustForward] = thruster.theBlock.MaxEffectiveThrust;
				} else {
					thrustPerDirection[thrustForward] += thruster.theBlock.MaxEffectiveThrust;
				}
			}
		}

		if(thrustPerDirection.Count == 0) {
			// Guess no usefull thrusters found?
			return;
		}

		float maxThrust = -1.0f; // Start negative so the first direction always has more thrust
		Base6Directions.Direction thrustDirection = Base6Directions.Direction.Forward; // Must have a dummy initial value.
		foreach(KeyValuePair<Base6Directions.Direction, float> directionThrust in thrustPerDirection) {
			if(directionThrust.Value > maxThrust) {
				maxThrust = directionThrust.Value;
				thrustDirection = directionThrust.Key;
			}
		}

		// use thrustDirection to set rotor offset
		this.direction = Base6Directions.GetVector(thrustDirection);

		foreach(Thruster thruster in thrusters) {
			thruster.theBlock.Enabled = false;
			thruster.IsOn = false;
		}

		// Put thrusters into the active list
		activeThrusters.Clear();
		foreach(Thruster thruster in availableThrusters) {
			if(thrustDirection == thruster.theBlock.Orientation.Forward) {
				thruster.theBlock.Enabled = true;
				thruster.IsOn = true;
				activeThrusters.Add(thruster);
			}
		}
	}

}

public class Thruster : BlockWrapper<IMyThrust> {

	// stays the same when in standby, if not in standby, this gets updated to wether or not the thruster is on
	public bool IsOn;

	// these 2 indicate the thruster was turned off from the script, and should be kept in the active list
	public bool IsOffBecauseDampeners = true;
	public bool IsOffBecauseJetpack = true;

	public string errStr = "";

	public Thruster(IMyThrust thruster) : base(thruster) {
		// this.IsOn = theBlock.Enabled;
		this.IsOn = false;
		this.theBlock.Enabled = true;
	}

	// sets the thrust in newtons (N)
	public void setThrust(double thrust) {
		errStr = "";
		/*errStr += $"\ntheBlock.Enabled: {theBlock.Enabled.toString()}";
		errStr += $"\nIsOffBecauseDampeners: {IsOffBecauseDampeners.toString()}";
		errStr += $"\nIsOffBecauseJetpack: {IsOffBecauseJetpack.toString()}";*/

		if(thrust > theBlock.MaxThrust) {
			thrust = theBlock.MaxThrust;
			// errStr += $"\nExceeding max thrust";
		} else if(thrust < 0) {
			// errStr += $"\nNegative Thrust";
			thrust = 0;
		}

		// Adjust for thrusters not at 100% efficiency, so we deliver the requested thrust.
		theBlock.ThrustOverride = (float)((thrust * theBlock.MaxThrust) / theBlock.MaxEffectiveThrust);
		/*errStr += $"\nEffective {(100*theBlock.MaxEffectiveThrust / theBlock.MaxThrust).Round(1)}%";
		errStr += $"\nOverride {theBlock.ThrustOverride}N";*/
	}
}

public abstract class BlockWrapper<T> where T: class, IMyTerminalBlock {
    public T theBlock { get; set; }

    public BlockWrapper(T block) {
    	theBlock = block;
    }
}



}
public static class CustomProgramExtensions {

	// projects a onto b
	public static Vector3D project(this Vector3D a, Vector3D b) {
		double aDotB = Vector3D.Dot(a, b);
		double bDotB = Vector3D.Dot(b, b);
		return b * aDotB / bDotB;
	}

	public static Vector3D reject(this Vector3D a, Vector3D b) {
		return Vector3D.Reject(a, b);
	}

	public static Vector3D normalized(this Vector3D vec) {
		return Vector3D.Normalize(vec);
	}

	public static double dot(this Vector3D a, Vector3D b) {
		return Vector3D.Dot(a, b);
	}

	// get movement and turn it into worldspace
	public static Vector3D getWorldMoveIndicator(this IMyShipController cont) {
		return Vector3D.TransformNormal(cont.MoveIndicator, cont.WorldMatrix);
	}

	public static string progressBar(this double val) {
		char[] bar = {' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' '};
		for(int i = 0; i < 10; i++) {
			if(i <= val * 10) {
				bar[i] = '|';
			}
		}
		var str_build = new StringBuilder("[");
		for(int i = 0; i < 10; i++) {
			str_build.Append(bar[i]);
		}
		str_build.Append("]");
		return str_build.ToString();
	}

	public static string progressBar(this float val) {
		return ((double)val).progressBar();
	}

	public static string progressBar(this Vector3D val) {
		return val.Length().progressBar();
	}

	public static String toString(this bool val) {
		return val ? "true" : "false";
	}
