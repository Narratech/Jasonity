+light_on(lamp) : true
	<- +moverALampara.

+moverALampara : true <- !accionBombilla.

+accionBombilla : light_on(lamp) 
	<- !light_off(lamp);
	+moverPosicionAgenteApagar().
