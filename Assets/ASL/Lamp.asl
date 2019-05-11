+!light_on(lamp)[source(LightOffAgent)]
	: light_off(lamp)
	<- light_on.


+!light_off(lamp)[source(LightOnAgent)]
	: light_on(lamp)
	<- light_off.
