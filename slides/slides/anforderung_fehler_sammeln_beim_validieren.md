## Neue Anforderung!
<div style="display:flex; align-items: center;">
    <div>
        <img src="./resources/business-cat_mirrored.jpg" alt="Business Cat" 
        style="width: 100%;" />
    </div>
    <div style="flex-grow: 1; display: flex; flex-direction: column; content-align: center; align-items: center; font-size: 1.25rem">
        <p>
            Beim Anlegen eines Eintrags werden alle Pflichtfelder validiert. Validierungsfehler werden gesammelt und am Ende gebündelt zurückgegeben. Nur wenn alle Validierungen erfolgreich sind, wird der Eintrag angelegt.
        </p>
        <p>
        Die Felder und Regeln:
        </p>
        <ol>
            <li>Vorname
                <ul>
                    <li>Muss existieren</li> 
                    <li>Darf nicht leer sein oder nur aus Whitespace bestehen</li> 
                    <li>Darf nicht länger als 30 Zeichen sein</li> 
                </ul>
            </li> 
            <li>Geburtstag
                <ul>
                    <li>Muss existieren</li> 
                    <li>Darf nicht in der Zukunft liegen</li> 
                </ul>
            </li> 
        </ol>
    </div>
</div> 
