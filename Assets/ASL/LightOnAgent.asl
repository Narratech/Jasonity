+light_off(lamp) : true
	<- +moverALampara.

+moverALampara : true <- !accionBombilla.

+accionBombilla : light_off(lamp) <- !light_on(lamp).