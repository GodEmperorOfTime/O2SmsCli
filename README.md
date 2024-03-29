# O2SmsCli

Utilitka pro testovani [O2 SMS Connectoru](https://www.o2.cz/podnikatele-a-firmy/volani/sms-connector).

Je to vytahnuty z vetsiho projektu a napraseny v jednom projektu. Potrebuji to kvulit hledani problemu a nechtel jsem prilis slozity solution.

## O2ConnectorConfig


`EndpointUri`: jedna se url endpointu, se ktery jsou odesilany zpravy. Je uvedeno v dokumentaci. Melo by byt `https://smsconnector.cz.o2.com/smsconnector/services/PPGwPort`.

`BaId` znamena Business application ID. Jedna se identifikator odesilatele (prideluje O2).

`ClientCertificate` je certifikat, kterym se autentizuje odesilatel. Vygenerovalo ho O2. Musi byt ulozen ve storu certifikatu i s privatnim klicem. Podrobnejsi popis nastaveni viz samostatna kapitolka.

`NickName`: jmeno, ktere se zobrazi prijemci. Musi byt ze seznamu schvalenym O2. Muze byt nezadano.

```json
"O2ConnectorConfig": {
  "EndpointUri": "https://smsconnector.cz.o2.com/smsconnector/services/PPGwPort",
  "BaId": "1992395",
  "NickName": "Firma.cz",
  "ClientCertificate": {
    "StoreLocation": "CurrentUser",
    "StoreName": "My",
    "Thumbprint": "dc937ad7b1c7874e138e5c32e90c798655920f14"
  }
},
```

## Config certifikatu

Certifikat musi byt ulozen ve windows storu certifikatu. V configu certifikatu musi byt uvedeno o jaky store se jedna a thumbprint prislusneho certifikatu.

Pr.:

```json
"ClientCertificate": {
  "StoreLocation": "CurrentUser",
  "StoreName": "My",
  "Thumbprint": "dc937ad7b1c7874e138e5c32e90c798655920f14"
}
```

Mozne hodnoty `StoreLocation` (musi byt uvedeno, case-insensitive):


| StoreLocation | Description                                                |
|---------------|------------------------------------------------------------|
| CurrentUser   | The X.509 certificate store used by the current user.      |
| LocalMachine  | The X.509 certificate store assigned to the local machine. |

Mozne hodnoty `StoreName` (musi byt uvedeno, case-insensitive):

| StoreName             | Description                                                                 |
| --------------------- | --------------------------------------------------------------------------- |
| AddressBook           | The X.509 certificate store for other users.                                |
| AuthRoot              | The X.509 certificate store for third-party certificate authorities (CAs).  |
| CertificateAuthority  | The X.509 certificate store for intermediate certificate authorities (CAs). |
| Disallowed            | The X.509 certificate store for revoked certificates.                       |
| My                    | The X.509 certificate store for personal certificates.                      |
| Root                  | The X.509 certificate store for trusted root certificate authorities (CAs). |
| TrustedPeople         | The X.509 certificate store for directly trusted people and resources.      |
| TrustedPublisher      | The X.509 certificate store for directly trusted publishers.                |

## Certifikat O2 SMS Connectoru

SMS brana komunikuje zkrz HTTPS. O2 ale nepouziva certifikat vystaveny nejakou duveryhdnou certifikacni autoritou (CA). Misto toho provozuji vlastni CA.

Aby komunikace fungovala, je tedy treba pridat jejich CA mezi duveryhodne ve windows storu certifikatu.

Certifikat jejich CA ziskavam tak, ze vlezu na jejicj [endpoint](https://smsconnector.cz.o2.com/smsconnector/services/PPGwPort) normalne z prohlizece, stahnu certifikat pro `CA O2 CZ` a naimportuju ho do `Trusted Root Certification Authorities` ve storu certifikatu.