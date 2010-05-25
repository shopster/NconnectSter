//	Copyright 2010 Shopster E-Commerce Inc.
//
//	Licensed under the Apache License, Version 2.0 (the "License");
//	you may not use this file except in compliance with the License.
//	You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0
//	
//	Unless required by applicable law or agreed to in writing, software
//	distributed under the License is distributed on an "AS IS" BASIS,
//	WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//	See the License for the specific language governing permissions and
//	limitations under the License.using System;


using System.Collections.Generic;

namespace Connectster.Server
{
	public class CountryCodes
	{
		public Dictionary<string, string> countryToCode;
		private static CountryCodes instance;
		public static CountryCodes Instance()
		{
			if (instance == null)
			{
				instance = new CountryCodes();
			}

			return instance;
		}

		private CountryCodes()
		{
			countryToCode = new Dictionary<string, string>(300);
			countryToCode.Add("AFGHANISTAN", "AF");
			countryToCode.Add("ÅLAND ISLANDS", "AX");
			countryToCode.Add("ALBANIA", "AL");
			countryToCode.Add("ALGERIA", "DZ");
			countryToCode.Add("AMERICAN SAMOA", "AS");
			countryToCode.Add("ANDORRA", "AD");
			countryToCode.Add("ANGOLA", "AO");
			countryToCode.Add("ANGUILLA", "AI");
			countryToCode.Add("ANTARCTICA", "AQ");
			countryToCode.Add("ANTIGUA AND BARBUDA", "AG");
			countryToCode.Add("ARGENTINA", "AR");
			countryToCode.Add("ARMENIA", "AM");
			countryToCode.Add("ARUBA", "AW");
			countryToCode.Add("AUSTRALIA", "AU");
			countryToCode.Add("AUSTRIA", "AT");
			countryToCode.Add("AZERBAIJAN", "AZ");
			countryToCode.Add("BAHAMAS", "BS");
			countryToCode.Add("BAHRAIN", "BH");
			countryToCode.Add("BANGLADESH", "BD");
			countryToCode.Add("BARBADOS", "BB");
			countryToCode.Add("BELARUS", "BY");
			countryToCode.Add("BELGIUM", "BE");
			countryToCode.Add("BELIZE", "BZ");
			countryToCode.Add("BENIN", "BJ");
			countryToCode.Add("BERMUDA", "BM");
			countryToCode.Add("BHUTAN", "BT");
			countryToCode.Add("BOLIVIA, PLURINATIONAL STATE OF", "BO");
			countryToCode.Add("BOSNIA AND HERZEGOVINA", "BA");
			countryToCode.Add("BOTSWANA", "BW");
			countryToCode.Add("BOUVET ISLAND", "BV");
			countryToCode.Add("BRAZIL", "BR");
			countryToCode.Add("BRITISH INDIAN OCEAN TERRITORY", "IO");
			countryToCode.Add("BRUNEI DARUSSALAM", "BN");
			countryToCode.Add("BULGARIA", "BG");
			countryToCode.Add("BURKINA FASO", "BF");
			countryToCode.Add("BURUNDI", "BI");
			countryToCode.Add("CAMBODIA", "KH");
			countryToCode.Add("CAMEROON", "CM");
			countryToCode.Add("CANADA", "CA");
			countryToCode.Add("CAPE VERDE", "CV");
			countryToCode.Add("CAYMAN ISLANDS", "KY");
			countryToCode.Add("CENTRAL AFRICAN REPUBLIC", "CF");
			countryToCode.Add("CHAD", "TD");
			countryToCode.Add("CHILE", "CL");
			countryToCode.Add("CHINA", "CN");
			countryToCode.Add("CHRISTMAS ISLAND", "CX");
			countryToCode.Add("COCOS (KEELING) ISLANDS", "CC");
			countryToCode.Add("COLOMBIA", "CO");
			countryToCode.Add("COMOROS", "KM");
			countryToCode.Add("CONGO", "CG");
			countryToCode.Add("CONGO, THE DEMOCRATIC REPUBLIC OF THE", "CD");
			countryToCode.Add("COOK ISLANDS", "CK");
			countryToCode.Add("COSTA RICA", "CR");
			countryToCode.Add("CÔTE D'IVOIRE", "CI");
			countryToCode.Add("CROATIA", "HR");
			countryToCode.Add("CUBA", "CU");
			countryToCode.Add("CYPRUS", "CY");
			countryToCode.Add("CZECH REPUBLIC", "CZ");
			countryToCode.Add("DENMARK", "DK");
			countryToCode.Add("DJIBOUTI", "DJ");
			countryToCode.Add("DOMINICA", "DM");
			countryToCode.Add("DOMINICAN REPUBLIC", "DO");
			countryToCode.Add("ECUADOR", "EC");
			countryToCode.Add("EGYPT", "EG");
			countryToCode.Add("EL SALVADOR", "SV");
			countryToCode.Add("EQUATORIAL GUINEA", "GQ");
			countryToCode.Add("ERITREA", "ER");
			countryToCode.Add("ESTONIA", "EE");
			countryToCode.Add("ETHIOPIA", "ET");
			countryToCode.Add("FALKLAND ISLANDS (MALVINAS)", "FK");
			countryToCode.Add("FAROE ISLANDS", "FO");
			countryToCode.Add("FIJI", "FJ");
			countryToCode.Add("FINLAND", "FI");
			countryToCode.Add("FRANCE", "FR");
			countryToCode.Add("FRENCH GUIANA", "GF");
			countryToCode.Add("FRENCH POLYNESIA", "PF");
			countryToCode.Add("FRENCH SOUTHERN TERRITORIES", "TF");
			countryToCode.Add("GABON", "GA");
			countryToCode.Add("GAMBIA", "GM");
			countryToCode.Add("GEORGIA", "GE");
			countryToCode.Add("GERMANY", "DE");
			countryToCode.Add("GHANA", "GH");
			countryToCode.Add("GIBRALTAR", "GI");
			countryToCode.Add("GREECE", "GR");
			countryToCode.Add("GREENLAND", "GL");
			countryToCode.Add("GRENADA", "GD");
			countryToCode.Add("GUADELOUPE", "GP");
			countryToCode.Add("GUAM", "GU");
			countryToCode.Add("GUATEMALA", "GT");
			countryToCode.Add("GUERNSEY", "GG");
			countryToCode.Add("GUINEA", "GN");
			countryToCode.Add("GUINEA-BISSAU", "GW");
			countryToCode.Add("GUYANA", "GY");
			countryToCode.Add("HAITI", "HT");
			countryToCode.Add("HEARD ISLAND AND MCDONALD ISLANDS", "HM");
			countryToCode.Add("HONDURAS", "HN");
			countryToCode.Add("HONG KONG", "HK");
			countryToCode.Add("HUNGARY", "HU");
			countryToCode.Add("ICELAND", "IS");
			countryToCode.Add("INDIA", "IN");
			countryToCode.Add("INDONESIA", "ID");
			countryToCode.Add("IRAN, ISLAMIC REPUBLIC OF", "IR");
			countryToCode.Add("IRAQ", "IQ");
			countryToCode.Add("IRELAND", "IE");
			countryToCode.Add("ISLE OF MAN", "IM");
			countryToCode.Add("ISRAEL", "IL");
			countryToCode.Add("ITALY", "IT");
			countryToCode.Add("JAMAICA", "JM");
			countryToCode.Add("JAPAN", "JP");
			countryToCode.Add("JERSEY", "JE");
			countryToCode.Add("JORDAN", "JO");
			countryToCode.Add("KAZAKHSTAN", "KZ");
			countryToCode.Add("KENYA", "KE");
			countryToCode.Add("KIRIBATI", "KI");
			countryToCode.Add("KOREA, DEMOCRATIC PEOPLE'S REPUBLIC OF", "KP");
			countryToCode.Add("KOREA, REPUBLIC OF", "KR");
			countryToCode.Add("KUWAIT", "KW");
			countryToCode.Add("KYRGYZSTAN", "KG");
			countryToCode.Add("LAO PEOPLE'S DEMOCRATIC REPUBLIC", "LA");
			countryToCode.Add("LATVIA", "LV");
			countryToCode.Add("LEBANON", "LB");
			countryToCode.Add("LESOTHO", "LS");
			countryToCode.Add("LIBERIA", "LR");
			countryToCode.Add("LIBYAN ARAB JAMAHIRIYA", "LY");
			countryToCode.Add("LIECHTENSTEIN", "LI");
			countryToCode.Add("LITHUANIA", "LT");
			countryToCode.Add("LUXEMBOURG", "LU");
			countryToCode.Add("MACAO", "MO");
			countryToCode.Add("MACEDONIA, THE FORMER YUGOSLAV REPUBLIC OF", "MK");
			countryToCode.Add("MADAGASCAR", "MG");
			countryToCode.Add("MALAWI", "MW");
			countryToCode.Add("MALAYSIA", "MY");
			countryToCode.Add("MALDIVES", "MV");
			countryToCode.Add("MALI", "ML");
			countryToCode.Add("MALTA", "MT");
			countryToCode.Add("MARSHALL ISLANDS", "MH");
			countryToCode.Add("MARTINIQUE", "MQ");
			countryToCode.Add("MAURITANIA", "MR");
			countryToCode.Add("MAURITIUS", "MU");
			countryToCode.Add("MAYOTTE", "YT");
			countryToCode.Add("MEXICO", "MX");
			countryToCode.Add("MICRONESIA, FEDERATED STATES OF", "FM");
			countryToCode.Add("MOLDOVA, REPUBLIC OF", "MD");
			countryToCode.Add("MONACO", "MC");
			countryToCode.Add("MONGOLIA", "MN");
			countryToCode.Add("MONTENEGRO", "ME");
			countryToCode.Add("MONTSERRAT", "MS");
			countryToCode.Add("MOROCCO", "MA");
			countryToCode.Add("MOZAMBIQUE", "MZ");
			countryToCode.Add("MYANMAR", "MM");
			countryToCode.Add("NAMIBIA", "NA");
			countryToCode.Add("NAURU", "NR");
			countryToCode.Add("NEPAL", "NP");
			countryToCode.Add("NETHERLANDS", "NL");
			countryToCode.Add("NETHERLANDS ANTILLES", "AN");
			countryToCode.Add("NEW CALEDONIA", "NC");
			countryToCode.Add("NEW ZEALAND", "NZ");
			countryToCode.Add("NICARAGUA", "NI");
			countryToCode.Add("NIGER", "NE");
			countryToCode.Add("NIGERIA", "NG");
			countryToCode.Add("NIUE", "NU");
			countryToCode.Add("NORFOLK ISLAND", "NF");
			countryToCode.Add("NORTHERN MARIANA ISLANDS", "MP");
			countryToCode.Add("NORWAY", "NO");
			countryToCode.Add("OMAN", "OM");
			countryToCode.Add("PAKISTAN", "PK");
			countryToCode.Add("PALAU", "PW");
			countryToCode.Add("PALESTINIAN TERRITORY, OCCUPIED", "PS");
			countryToCode.Add("PANAMA", "PA");
			countryToCode.Add("PAPUA NEW GUINEA", "PG");
			countryToCode.Add("PARAGUAY", "PY");
			countryToCode.Add("PERU", "PE");
			countryToCode.Add("PHILIPPINES", "PH");
			countryToCode.Add("PITCAIRN", "PN");
			countryToCode.Add("POLAND", "PL");
			countryToCode.Add("PORTUGAL", "PT");
			countryToCode.Add("PUERTO RICO", "PR");
			countryToCode.Add("QATAR", "QA");
			countryToCode.Add("RÉUNION", "RE");
			countryToCode.Add("ROMANIA", "RO");
			countryToCode.Add("RUSSIAN FEDERATION", "RU");
			countryToCode.Add("RWANDA", "RW");
			countryToCode.Add("SAINT BARTHÉLEMY", "BL");
			countryToCode.Add("SAINT HELENA, ASCENSION AND TRISTAN DA CUNHA", "SH");
			countryToCode.Add("TTS AND NEVIS", "KN");
			countryToCode.Add("SAINT LUCIA", "LC");
			countryToCode.Add("SAINT MARTIN", "MF");
			countryToCode.Add("SAINT PIERRE AND MIQUELON", "PM");
			countryToCode.Add("SAINT VINCENT AND THE GRENADINES", "VC");
			countryToCode.Add("SAMOA", "WS");
			countryToCode.Add("SAN MARINO", "SM");
			countryToCode.Add("SAO TOME AND PRINCIPE", "ST");
			countryToCode.Add("SAUDI ARABIA", "SA");
			countryToCode.Add("SENEGAL", "SN");
			countryToCode.Add("SERBIA", "RS");
			countryToCode.Add("SEYCHELLES", "SC");
			countryToCode.Add("SIERRA LEONE", "SL");
			countryToCode.Add("SINGAPORE", "SG");
			countryToCode.Add("SLOVAKIA", "SK");
			countryToCode.Add("SLOVENIA", "SI");
			countryToCode.Add("SOLOMON ISLANDS", "SB");
			countryToCode.Add("SOMALIA", "SO");
			countryToCode.Add("SOUTH AFRICA", "ZA");
			countryToCode.Add("SOUTH GEORGIA AND THE SOUTH SANDWICH ISLANDS", "GS");
			countryToCode.Add("SPAIN", "ES");
			countryToCode.Add("SRI LANKA", "LK");
			countryToCode.Add("SUDAN", "SD");
			countryToCode.Add("SURINAME", "SR");
			countryToCode.Add("SVALBARD AND JAN MAYEN", "SJ");
			countryToCode.Add("SWAZILAND", "SZ");
			countryToCode.Add("SWEDEN", "SE");
			countryToCode.Add("SWITZERLAND", "CH");
			countryToCode.Add("SYRIAN ARAB REPUBLIC", "SY");
			countryToCode.Add("TAIWAN, PROVINCE OF CHINA", "TW");
			countryToCode.Add("TAJIKISTAN", "TJ");
			countryToCode.Add("TANZANIA, UNITED REPUBLIC OF", "TZ");
			countryToCode.Add("THAILAND", "TH");
			countryToCode.Add("TIMOR-LESTE", "TL");
			countryToCode.Add("TOGO", "TG");
			countryToCode.Add("TOKELAU", "TK");
			countryToCode.Add("TONGA", "TO");
			countryToCode.Add("TRINIDAD AND TOBAGO", "TT");
			countryToCode.Add("TUNISIA", "TN");
			countryToCode.Add("TURKEY", "TR");
			countryToCode.Add("TURKMENISTAN", "TM");
			countryToCode.Add("TURKS AND CAICOS ISLANDS", "TC");
			countryToCode.Add("TUVALU", "TV");
			countryToCode.Add("UGANDA", "UG");
			countryToCode.Add("UKRAINE", "UA");
			countryToCode.Add("UNITED ARAB EMIRATES", "AE");
			countryToCode.Add("UNITED KINGDOM", "GB");
			countryToCode.Add("UNITED STATES", "US");
			countryToCode.Add("UNITED STATES MINOR OUTLYING ISLANDS", "UM");
			countryToCode.Add("URUGUAY", "UY");
			countryToCode.Add("UZBEKISTAN", "UZ");
			countryToCode.Add("VANUATU", "VU");
			countryToCode.Add("VATICAN CITY STATE", "see HOLY SEE");
			countryToCode.Add("VENEZUELA, BOLIVARIAN REPUBLIC OF", "VE");
			countryToCode.Add("VIET NAM", "VN");
			countryToCode.Add("VIRGIN ISLANDS, BRITISH", "VG");
			countryToCode.Add("VIRGIN ISLANDS, U.S.", "VI");
			countryToCode.Add("WALLIS AND FUTUNA", "WF");
			countryToCode.Add("WESTERN SAHARA", "EH");
			countryToCode.Add("YEMEN", "YE");
			countryToCode.Add("ZAMBIA", "ZM");
			countryToCode.Add("ZIMBABWE", "ZW");
		}
	}
}
