

var keyboard = {};

//--- key codes
keyboard.key_code = {

	a : 65,
	d : 68,
	s : 83,
	w : 87,
	shift : 16
}

keyboard._key = [];

keyboard._onKeyDown = function(event)
{
	if(keyboard._key[event.keyCode])
	{
		keyboard._key[event.keyCode].pressed = true;	
		if(keyboard._key[event.keyCode].onPress)
		{
			keyboard._key[event.keyCode].onPress(event.keyCode);
		}
	}
}

keyboard._onKeyUp = function(event)
{
	if(keyboard._key[event.keyCode])
	{
		keyboard._key[event.keyCode].pressed = false;	
		if(keyboard._key[event.keyCode].onRelease)
		{
			keyboard._key[event.keyCode].onRelease(event.keyCode);
		}	
	}
}

keyboard.Initialize = function()
{
	document.onkeydown = keyboard._onKeyDown;
	document.onkeyup = keyboard._onKeyUp;	
}

keyboard.TrackState = function(keyCode)
{
	if(!keyboard._key[keyCode])
		keyboard._key[keyCode] = {};	
}

keyboard.RegisterPressEvent = function(keyCode, callback)
{
	keyboard.TrackState(keyCode);	
	keyboard._key[keyCode].onPress = callback;
}

keyboard.RegisterReleaseEvent = function(keyCode, callback)
{
	keyboard.TrackState(keyCode);	
	keyboard._key[keyCode].onRelease = callback;
}

keyboard.RegisterPressedEvent = function(keyCode, callback)
{
	keyboard.TrackState(keyCode);	
	keyboard._key[keyCode].whilePressed = callback;
}

keyboard.IsPressed = function(keyCode)
{
	keyboard.TrackState(keyCode);
	return keyboard._key[keyCode] && keyboard._key[keyCode].pressed;
}

keyboard._callPressedEvents = function(element, index, array)
{
	if(element.whilePressed)
	{
		element.whilePressed
	}
}

keyboard.Update = function()
{

}