namespace WebChatBuilderModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedHideHeaderAndHideSendButtonToTemplate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Templates", "HideHeader", c => c.Boolean(nullable: false));
            AddColumn("dbo.Templates", "HideSendButton", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Templates", "HideSendButton");
            DropColumn("dbo.Templates", "HideHeader");
        }
    }
}


/*

ALTER TABLE [dbo].[Templates] ADD [HideHeader] [bit] NOT NULL DEFAULT 0
ALTER TABLE [dbo].[Templates] ADD [HideSendButton] [bit] NOT NULL DEFAULT 0
INSERT [dbo].[__MigrationHistory]([MigrationId], [ContextKey], [Model], [ProductVersion])
VALUES (N'201601311921342_AddedHideHeaderAndHideSendButtonToTemplate', N'WebChatBuilderModels.Repository',  0x1F8B0800000000000400ED3DCB72DC3892F78DD87FA8A8D3EE8447E5C774478F439A095996A76B46B2B4963C9E3E29281292186691D524CBB67663BF6C0FFB49FB0B4B90208947E28D62953C1D3E5845008944229148201399FFF73FFF7BF8E76FAB6CF60595555AE447F31707CFE73394C74592E6F747F34D7DF7FB9FE67FFED3BFFECBE169B2FA36FB7B5FEF15AED7B4CCABA3F9435DAF5F2F1655FC80565175B04AE3B2A88ABBFA202E568B2829162F9F3FFFE3E2C58B056A40CC1B58B3D9E1874D5EA72BD4FE687E9E14798CD6F526CACE8B046515F9DE945CB55067EFA315AAD6518C8EE69FD0EDC94354BFD9A45982CAAEFE7C769CA55183CB15CAEEE6B328CF8B3AAA1B4C5F7FACD0555D16F9FDD5BAF91065D78F6BD4D4BB8BB20A9111BC1EAB9B0EE6F94B3C98C5D8B007156FAABA5859027CF18A5067C13777A2F17CA05E43BFD386CEF5231E754BC3A3F9F13DCAEBF98CEFE9F54956E25A307D0FC87F6DE36733A8CAB381391A1EC2FF9ECD4E3659BD29D1518E36751965CF66979BDB2C8DFF861EAF8BCF283FCA37594623DBA0DB94311F9A4F9765B14665FDF801DDD1435826F3D9826DBCE05B0F6DF986DD489779FDEAE57CF6BE4123BACDD0C0151455AEEAA2447F41392AA31A2597515DA3B299D465825ABA0A28701D367C7D97DE8F3D36ACD8ACABF9EC3CFA7686F2FBFAE168DEFC399FBD4BBFA1A4FF42B0F898A7CD326C1AD5E5060158AA7B7E9B56EB2C7AC43F76D6F97215DD87E85DDDD9CF51751CD7E9177492A5CDAC9CA531CAABA1DB374591A128B71EC2394AD2E80C7D41998E5BD4709615412879579478D15823F63EFA92DEB7DCC873733BEA65FEA9283FDF97C566DD88C10F286BAB560FE9BA9386DD9ABD812ABF2B8BD58722EB170650E7E6AAD894312666A1A9781D95F7A83647FEEA739A652A84FB0A3C92DD770962A4D016998F759AA5FFD9E1A14089ADC62346974AD063AA40481E2E4679AD94E203E55D25F90060E7D27CC0C445A2338D7F93EA5BED7C49E4ACAF68C570125F208DD4FF8F0DDA8440A6AAD2FB1C37F1DE33A2F2732BE7DFA20C75EA5B58597F8E56B7CDB20C2FE67B59A415F3BD5C33C5BBF9799766084679807B33561B91154B05A90A54F192AA048AAB4C25CD772E51091E2EF2946A3A9534DD918A8AAAB84CD7F42ADD9E868AA2860DAED1B77AA2AECE8AFBA2998787AD77B7CCE36C93A0E6905DBE8DEAE8B83A6917CE32BF2BBC05330FFAB86E4671BBA9B194F0037D5222CCA917C3CC371DA0EB7465CF456751557F5C27AED0A4629360F8E611949B6491DE50B546B129140A5253AC61AB2F53C3D6A0C8D514D1642A4851656B053C6BF41D88A70DB6448A99E389E31AAD1A45AE464AA4C64A225A7D9914B1A1822D6A9FD2A4ADAF40ACAF22A2D595489122C5D6288DA79D106A04AFED28340D2735E278BD6EB6F1164D2CBE9C2FDB58303B572B5CF409E521E9C5CB9FB6B1B5BF4BCBAADE8D5681E5D44EAFDC76D4792367C86EE2BDA5E28644D47BC30A75629DF43EF33AADB3EDF7B28C3BD1B4E56E4E57519A297A79F9C38FA17A696F65CA15F2BE5AB88CAAEA6B51263F47D5F635E82B146FCA66FBB8AAA3D57AEBBD5D3E14397ABFC1B70853F6156C6AAEBF16EFA2B8396F9EB61736DEF0CE8AF873B1A94FF3A4153B752C8A1C430041D0398E635455EF1A6646C949B1C987F3A29BED01AF70CD8660BAFEDC4F325994AE243756AC7E73D357A5EEABC01AE2E53A5CCDFA5453DCA7B2FB7FAE87BEAA1CD5AE86165552CD16550CCC0C5352538E685B418B6757CBEB72ADBF0FC2F0DA1952E9C5E78381FBB85ABF47F541DFFAA083FBAE6C603642FAF38100F6D9CCB8F1A83CBF34559E5FBDB8BD7BF5D30F3F46C9AB1FFF805EFD30BD22BDFD1B394CCA5D28EEEDF4E14EB7BE37B53DFD3DCA36A1BB725A0DAD1008BF1A5AB0FBBF1A5A349BCF5FD2046B250B7D8BBE7203DEA87ECFCFB66B8EC36CEAE5C00C73EACEA791014ECB05EF45E1570B86BAFF8B056665B02A1E900BD7EF4AFAF7F8EE09C7B517BBAE57776DE39D5FD8B558B8B0C0D0F0378F8A6D7B543C09E783164994B45E011E3E5BB097016760313E58B62CD32C9B44E963606CE0E19193D87F9C64C968C87113277DFB9D4B941E1117A142B79D4AAE84BA4BB53DC8B4B37C52555B3FC8102B7967F6F7F6A9FA6EFD14BAEE9671EBB819C42FE1BA8CF2CE832410C0CB66E8DEB02EB328460F4536D5345EA13C79B3A9EB22C7B49DA43B422EBA3F576A5D3D145F97798A9F8578B3C539AAAAE81E1D9765F1D51B1846ACBB7DF65CD06930D1D0401A27DB0E9AB95333712B70F4686E5BEF7C8BECD070F2651E5A3E31D73BCDB90EC58F7186687E7633675CD55159FB83397940F1E7464FEEB559BF85F101FDBA49CB5E373EFE12A519ADCDBBA1D810FEB4AAD3159ED44F511A60D058A25CE4E7C56D1AE0A4D14269B875DCB89DED53CBBB32F2977397C5FAE20B2A9B739C2F248C52809D05C398C601B3E928C04C9C459B3C7E984465B82CAA7412AFDB0FC5264FFC6DB2CDC651A77194F9C27913C5AD93591EE2924373EA69E6F1A4C88A096CFC58DFFCB9D537271C1F1666674598134F7AFFE069650FB0FC2EEEEE2A54FF2304905FFC805C3784ACD3F5249280F4350DA7B62A75D76110EDBC03755CB74A4908E1BBCCDFA3AF9FD23C29BE86786015E6A1D6156AF697242A1FAFEA4707C5417A79D7A85F2BD897B77310EECA2927DEF1B3F808882AF37250E83A753B7CE0B63B3F7A60245C0E1E7DBBA98E1DB8BF5DF9E7DEA26C1A811348FD18B7D569D07E53940976D89BA4AFF63261CABEA6D38CC6FEC277A514ABEF529425B02DA41591749D51C072458290E5CBBD056D0BC747DAB600F642E4B698B8CADDA1F113BBF37111BC531E47B6DE573B718CB35ABBC4C6AFB65A25CA13EA91A3B72BF17ADD00BC2E023EC8ACB0670859B7BEF735EC1D80E3455E6765CBA2AA42A12D6DC6B28C0AE9E12ACD28103B9767142E4EBE436CF3A9645A1BAC47588BD457FBFB57D131C3F18E73A4C8256AB6543B9F7E6336EC220BB9F11F2ED839E3E1BE5D38AE6F37D0F4C73F6C95D5AE505531DC3DA1574191E728AE43F5AEB39260BA629ADCA5533C168AF02D6BBA8EE8F86C5BF49A68263D9A8A946D9C9A494C6DCA7798FAA6C779F515956E6D4FE9C3AEB4A13C021709F3009E5B302FDE8C35C6530B53209C59D852AF13CBD8B9E3164FDAEF5CCCF688B83A06F76DA712B778AE3F96DB3F392CD7C74952A2093CB63015490CCE6D8BF096553BB6DD8D72DC61405C639CD5131AC8CE1710838D93C2C2037862077FDD2131D053D88169B66D84E24F0DCCFC989D1E74D1CFE04D8DEEE766A8496D6E50057193036B796D7667517EBFF158B07DFB9DAFD51E11A7B75F54DB27B642AD8F15C9D67BB57EE715FE8DD7D378DFF55462EC685875EB6105B016051B39E8F9BE21D546A12A960A1215A8E2254EAF505DB7F4717C4AD635DFB9302578383D271B9B4E254AD56F66B72449A77D50BE59D126B5D1EEB0ACDE65D1FD98E3C0D1C4D6030CCB78CD501B18D963431A9A1558B276616E2967B6F9AC25EDD1FCB930054CDD3E72CF50FF85BA7EEB0A7C5B7C1BEABF54D7EF23E790DAAFD4B5DFB62FBE48DD3FA8EB923849A4F20F9A61D2517C48931F45B6E91844C134D4057918A61900EE9A6996F997284B1353BE3989B2CC9C67A2DA945FD859D5B04B2BECD2D8946330CEB751FCD994697090C3658D565E1C031C8EC2708E0078D71C442EDB91310FB5773A7F2DD21C198B9FB6C9DBB48ABBBE4CB96A9947AD7B1FD052C3621F5085DD1551C230B186D12EA34DC5B5D0B05AD3CD66C53531E2B6E3AA2AE2B4659BD18C0DC588623B3FCD939959C0A8F1982FC4E83CDF6087CEE65BB3D94233A7ED6478403C7602843C62BBF99DD04DA370A112B78A7040BBAA61E934AF45ED2CCDB1B124331A36D7DAF0DC81E768E8872F798BB0774183A611494C109006B6580C7D715AA78E52870B8A9DECB88C84F73265003ED6D756B88C8B1426E132124A68122E6387BD032E6349F2E4B8AC0BCD663AFF5C9CB6ADF0181BE54DC262DDD5CC241CC68C79070CC6D063EFF9EB52080B2F9B77A126C457C395B46EAA5570015ED2F0EBF3838317BCAE604D0236ECBC0E5D490CFA20A48023D74F4C0E12FC44872B1FEA3E0801F8F02A235012614909D261B043B0151D666208FD20031602EF5360C7403001679977EB962128F5F1A6DC57DB27303ADC9450815153FEDEA1269B7E2724C30C7C343462D5471B309F61E8A59135F11C389A60AA633C3EF742106EE6323668E9E73560209383742E14D9A1281CC7C411564CAD4A1A614B561B0D5196C34BAAC069F336521B4DE77462CE02FA6C610684F6258666EB8273427A0F7AB24D0B482CA9C60CCE32A919B086DB15C92929C88CA77BA8F1B3AE7D3204257E7E94D3467BD5643ECBB06B203DD8C1E12FA078835D3EA438AAFD3F241E2B565450FA8EE8059D1731003BAD0C4F95D1563CA342E75315F72BECBDD39F80E5439DE0F82BA78449E770BCD9AD9C7D3B9368D3A66E5A50D7CE6BFCAEA8281F012B44331C6288A8889995E7070CF20AD5B460AEE6B3D1FACA0A58819DD8E674FE4D0104B5596AC00C6A90086458931A10C405421C077FEED4C011EEB221A0C085B705D83EAB82122CB9E1B4004B522028A1764B5A03B48F212A40224A81A6797FF883208C07431D63B55A37C855441FD700C0A714A879777A3168DC3F1E062190639E8E279944DA022046D3D080C23B3904A3D3084CD646B7CD8B480C1A800E037A17055161F7670DB8DEF11082347A666A80E858DE8CDD3BB71D90E17B9F260E0425AF29512AC9F3425586651268EDB3B1F70D231AC4A0B039199BF528589028E47DA2584A3850A917867A2A41D62A1B7B95179538B394844AFD6082538930BA9E4880B1C5C2DCE24522D6AA22A11019883781442300401C8DA5C0D056408D64D4531474915B0774E4F5A0039780564E0B85C9C0C268E04A13D84CB045BAF4BA8C9C20D0CD8B89D9C09504DC9D0B05A6C735D8E0476D4B3E7CD88C606648702581603AA00051DAA3371D844030221994460523B302853BD139151490591138203DBEDE1460E28C89A3979A17B406060AE14155578C1B3229E808E7C1F8FD1941CEF690ADC1C4DAE0CAF29C7D414F3E97C90612440373AE313E989A1FE82150B7002A26905B1B0CC8EAA245C9EC0C9022656493B0B34A50A3EAEF5954FA94CE0E6144716732C9774AB985426FA37021C1043B24608F900E5C6EB530B55BB810013453D08A1283BB3741B8D803222D14160C031B068538B9C9501000B65AF04A227869E13070F87D2A4000BD55C3C2AE411384BB5851514669C9D88E10851E9A89D4D199394C0D1DD418C0A3A2A94923D0F1B37F9634DCC10F65878BABF801AD22F2E170D15489D1BADE4459E7C3DF179C47EB35BE5E1A5B922FB3AB7514E349FDFDD57CF66D95E5D5D1FCA1AED7AF178BAA055D1DAC86679C71B15A4449B178F9FCF91F172F5E2C561D8C45CCD099B7180C3DD545D9700C57DABD897D9796558D17D36D84DF359C242BA11A6571905CA5F51D31460571E2FAEBB5BE3AFE9BE82ED0A389CE06211A6548F377CD9856D8ACD33EA2E385ACD8AE697915475954F231E470756CD23929B2CD2AE73EF2FC2787336677A3018D5FCD2131D9DA68604C8135BCE5AABD540500921273883F4724F8F2499636843A4B6394571C6C591DF35EDA875667E80B8EA94843A6BF9B435B5604059C3EAEBB38A78102C522ECC305C77C821550E0724ED6F06BC6684529F551CB55359AE5EC5796A2AD8CEA43137E613005DFC32A1BE391B36CD57FB583948860AC46D7ACBE362297B026C9572B6CA828811C525489C5BA16933832CB5B2CDE9B95A838345BAE43890A69B00AA52D65F4260DF845437D369F3B71B958EF46A8CB2827CC3B5360C1E9543E3F86D7A9EFB6D0C6947D22C4B1CC620D75E9DCFA53CD714507AC659694AAA2477F43C4DD4AD31F5DD1422A77C68E0B6E46A9CFE6B02823010F8F2BDA1BA9203BF1D82ABC9CB38A83EAAB832065187EBFB1120AED2142940CD467BBE917418D5F77A846D37127197874811D3CC2CD22BCA160170A48F803C37597A29606453E598C2FEE5CB898D1C5905B970A0A89564003219F2C61B43A67B9E2278F2F3387DAC7F56894346EE3614BCC215EA1785362D95047AB350B922BB2C0920ECBC120491738C1935014AE61C17B5F8B7751DC48CA21681FC38642A985AC2AE2CFC5A63ECD9376C9D63127B4C46207D800CE7C9939D4E3384655F5AE6151949C149B9CD399806273D878258AC276FCBA37DB35E0D9E4BD778B3EA1F6BBB7018CEDECDFFD835E7EDE2C8FE618E52E5C0AA3048E9F2D619148530230F27D2FF949EAA6E6C14F9D33B01F3F4960C8A54F53BDF9F6254D7839CF1559487AD2A68D5AC61D44C7826D71ED1EF086CC3BCF83355A17573FCE80416C5754F4AF286828B297153B9B3AA9C9DB72BE3ABF79FB4992B4932A7BB83A4FD4E1E37772C3FADB4D64CF9DA3A79C3783CA9C000D7854DE54AA8D93163C83D1DFA73D57920C4B157735467DB6BE78EB6E0AC1ABB6BEE8FBBAE0EC5AE23CDD1504921458D3F1BA8CF2EE5218A4255D6C0D1B3F1884C192120BCD0667647B6833B289D32414DA9CDFF3A44BB1D8A56B670FF06C991D543252182C536801F7A1F8BACC53FCD0916302B6C4C600DCBABA1C9765F19503C915D961D9656DE1318472B928793E85173AFDDD0EDA38A72244BA6C6F36A1DE7BD5DF2A0D3AE49A98A4250DA5F6E8B6BE608C1EBE4E6909FB80E2C738432237320516AC8DC35E02BC3D7EB6D81471D0E046F3E93D16999D912BB319F1AF9BB44449D7F2F84B9466A23227AD64A3D77D3BADEA7485AFD03F45294014B8869D18B9C8CF8BDB94479F2DB1C0B96DD17022BF2B330556C7E3E55D09DDC7F59F2D36B8627DF105958DCECEED6DD4773BCC848D67F86863102872518919BFDA4102284F7DB63160E144F7A21E407FB7A17C9F4494A57BFFD5E6CC4D5275B3876EF2D11CCEDF114E6217713694F1AB39A431EB370B8BFE6E711418B35EB3E78AE1B3A542D7A5D8952129A9622741CE0A517DA0BFDB28E0E9FD837040E8BE994301D68035FF5FDCDD55A8FE070B65F8680BE71708CE2F567C5114388489B82099026B7810AB3125962A69D716D04AFB022778C775BBFB4BC10EE5B6F26D99BF475F3FA579527C85C41C53BC1B1FBA2BD448EF242A1FAFEA47F1BE882FDD1B95BA7B0CE7AD50438FFA0CD469B899D4EFA3A9CDABD2FD370BEF91A685A84C8F5F6D58F31665C0C2A4BF4FBF618EDB03809A506801B728136C121761D20516F0DAD325048F2EB08527D996854217B832907BB69CA58F8B1DD674177AC66D614BDAAAD665DB045AE243C19407E6761D034BDB51B5932A7476BE6F434E22D6F76DF86C773347B9608A37734CA185D7C71A8726BB2E645EA750B9CD568B0D80242612BBCB5205BB38F2902BFC2CAA2ADEF1952BDA1B79A1796E6A2931E81853F63243D95A7AB81F1B097662B6C8F22190B8C6A8CF565743322B22576471A1310EEC129531E2ED0A50F9DE705CF722D79BD5DA2753F63C063793DF4A46C2DD6DFFCD46CC5615C09ED4672B6B39CE4D0480634B6C6E5EF178B07FC65DCAEF517C998D7329BE9F49D791F8D6912BB231A4D5A88CC0C17345E630DBE749A2A6407DDEADC3F6715E7D452504702CB183782A1E32A8CF7B2326C657F7FEBB92248E80C996246DAAB86C6E5B404E4BFD770B6DAF2C561F4B4E071D3E5A2C9DF571929488F739A03EDBF9C19178ADFC00C130AE2A589D5AD4055710D52528E842C752BBD9B9D8D009FE5B1813C2D2612F53B757D39C34137611BE70CAF31695E79ED5EB1D3CCF87C8A08015DFEAFA555003651AE08EB8720C6DEACD904324547B5E9437951FAEC75CF0EC095B965F5E05CD9FF9BA8CEDAC4695ECD3440776E7F574E5B573E3F57B2F603EB9BB72DDED83FA7ACF0E81E4E2BE2B6B293F960C09C4D963892425B90A96E0636FE95B0F3CBDD8D9AB0B363C0FF81E9F0E046BF6EE9E6E61F8C01E871E129F34C0C15E4522192DC3110E9C6380C2C00539699205D97B0DCBB0014A049BF370D2DEE0CD96154E393FE4523625001FA8C99959B868B9860CC3B5F2671A301AAE23E3B0B002320F1800788F19484788604CD4072334E49EBEBA846DC0B71E8A6961E32CEE663EB4D8D9B1B4DDFB153966C166788C9F6C38C763037FE1C007493624A2EE75C58DEA9985E36CF381A1F7583A1890C19B7D84B0D306565DA6BEB1E916981349986AC32991F973DCC89C3A2C394612FFDA023B7B7BB43FD32828E0CD2C4C846E231F7952D7CC0F1E98032016F79E70071068DC10335BA7FEFDE6895EAEF661CC0D779FBEBAFFDEC3462B0F3007378E53A14571FF771C0D01FC050810F5DD340224D5C49D69E431E24D19471B21F246192BD256CA48C3D0EF33179990C29B97E401F34D22F5820DC1D8BCD230A2C074E9A2EC1B4E9A5D905F4B96D285EE0FB610263E08B199018C784076CC6D4B4D0FB9502A81FD99E7A77DB885521F18CD2CDBC4C2554C4A48285982F734DF849D6C280F83218EEE8E70FEFB859214DE3CC4658B30700CA36A9B388001D3016685309D098D0BC88DCA17C4926DC004188678DA78B7F9F388C1E8FD39054EAF61E38731B6B272B780A646953E2388F2777319EC2A55993FC4949FBC1C4BFC394C4B153D7F091938F82A83F1917C197E0F193848F60B262D474B319C64A3A554453271F0E930BA2AF3D925894FD6ECD88F558D5607B8C2C1D5AF5997D960AC701EE5E91DAAEAEBE233CA8FE62F9FBF78399F1D67695475795248A28FD77CDE6EA3CC1F2F5EE1CC1F28592DF8E6F6F9433094AA4A18658CB28DD35B2097F3E36F48E0859E47C624E9D28D876F7D28DBD7BB2CBC6D189676D9FF0535738F8D3A97515DA3321FFD1EE633CC7FD81D69E0C185B28F319255D749FE252AE387A8FCB755F4EDDFADA131A1AC02022411763510DB14EF1A80B23C1C1DE8DBB4B64691CEBF414D94251420E186294AB44781928325274D332E561EB5759CCC34FEE7E5E6F1E9AE3BBB91DC17EE00C6AC175E48504F433CD68E18522E38D7839777663CAFD8AA751C4F35DD22BF0714B474BA891072960AC7160CDC188B2D00486572098FA5A1CA21E10E964A1ED101C14E1D751BA6C712129736C2131AEBA2C44ECB7CC6EB66AF977982BE1DCDFFAB6DFD7AB6FCC70D0DE0D9EC02BFD57E3D7B3EFBEFF9EC3CFA7686F2FBFAE168FEE2E54FB6D32BBABE3820C703098920687DA76485017A000816433B8C04EB8C1D365C732F4CE01B7E4B7C2020E658196F30CA3423661B8DCB0E03B1B49C238D163395A224C49E32A629D94B6D8C79F7E629069924259EB0426889C1CF4D248E6C887D97242909008AA42A51AC82973FFCE804944AADE13E096CA29200E3E5D2940480C8A429090B2F0809C57C24EEB0800424F2956AA462708947DC5103328DB85F258C39466C359EBE65FBF9D96C597DCCD35F374DC175430D4EE3E15696CF094D93E9638A2D742BA7B33E138083E2D935355537CD34F531F34880854EA71EB106E7C41940CE0E33CED0A4E880CFFDF2141C407D38E5839EFF38CC026B73CC2802C3DE256B3B718FF820D08C7964D93C80AA70CA0E3D17EC9798E847E1804DD7748A99057C67CCA653E264A29FA4A1E13FF5EDF593B9F485337198F188EA35848E4D247739A139C5EC3466C67463628F7097AA7DC07F0F2BC5DEDF1F33B93CBCEF8BE9E41DDEC048CA0E8F932B9FA523C8E1954DD1110824939EC37DC46C520E1F6B309389C30FA12E0CBFC7224A832C452EDF4678BB34F05AC7D0282D7D48A0B5484397DC3B31CF99303A9302C3FD6680CA7DE10E84CF77E1CE58D2D416EEC8C1892C3C28C6A4AFF0100B74D60AAFAB9D3E5D858770A7D254B84319325478EC56436E8A20D7CA43760A77FAD2592942DC860E515A43C8803ED6B63BC1C7B414EE30E8F40E21EC0A63CCEB100487D350845035A83C143E5A6D9783C29D43BD597CC83DE10BE2171F104CB689106CC4249B0834DF43B2094F2D8E4B2EE12B9E986C127E8E64FECE687CE6087758CC23623B4B3FD5740BF67D310B8499722A7B10AD534D011A84564CC7741261ECFC634289FDD8E8843C1201D062F248848047E79108062FEC7E4A2791D89E954712D2C37C9549C25D982DB5A1F1AE0F82E66B2DB4AE14041E95E3C1E3BCC5A775F0B0A903691C7C763A2A7B83CFD98B3D11381DC0D9640D21A66E5FB75EE9435A43E39DEA65A981618E6DBE45F940E56EF0BAFB100D344EB70C4082067768DCEB5B3BF6621A6F81C1C4F7B6669C053F44D5B354DFAE9720F75B61272A774310131193BB2184F30897B821C46EC6A66D0862CA629236048048A56D0800CDD07DD514D498B0C11FD629ADBFBB02025F64B3EBC6D0438283B28D7D0A4CC460EE61227B706EE23A029326F889B1CFEA1062658D791D0240A3323B84104D546A87ED9D78148FF50DF71FF57B75838D8807B0EBD38F11E354017C6D872C0B21AE1D3D55B64B3126809D7E2400D8827083932718FA5E4AD325187847526D77CD9D867A52E204C9DAA1D1D599714F9E194114D7733B1E34FE6B62E774307181A1B3A12C578181BBE1D8748BAC0F7806BB71FED61DC12FA581C13D5F72C313E7EA25EA2BC7E5F2DBECDC0738A91A744E9A59746DFE365211E1D02B7A0A58D72B4685CF0D85E7BC8578E4BAC5F9735E753673E7BCE676386F3B5F6F54C0A8F1D69B7E8C7CD3BED3E132FFE0005BED762ABE5D2623C1E1990E84B2F30DB66337DF1A4C9A9D5FA0CE08567CD14503064A59D0BF1340377C824ADC2AC24F33ABBA8C5231B30E767BC557409964705C7D431509537F80CC97BC45D8C8D020261DB849A7D247268B013EB72874F4606289D9714CFB2869A71CD3BDF492710C29FD1E390678E2F614380648FD362DC3B4E72119BF7485DF23BB4872E4ED19B7F45714638AB12D70CAF3830315B35CD2B13609C4E1DBF7C21A9770947AF8FE50935A6D42BE603389FDC61B3BE70D93EC6913F28722F2B9DB2CBE10A2E35EE4ED5344343B8EBB00A427511547627AD53608AC0683EEB004A0410AB6C25136D3ABBA0AB166283088BD7DB7D3B31314E8989A4C7116C1E9FB0E59C97C461501FBB7C7468A4E276422C9F3E27106D96C7964F6C68F7BBE41C1C393CC883A87DE567628C3FC7D1330049FAE4E3687634A333281DD072B3E18DD647940E4EB567841F47E974C8A3CEF99310F4873FB493B54645B9B60F6E91C74DB9C793AF71981D27F7AF2730EA7EADBD309EFE53FF862D961BEF64EF25BCC862A8BDD56A4BE41F6BC29D6BC982F4ECA0683DD8BE184F1EBBE33832C33193C3DEABC5CDB6109D3A47853DC8ADAE466A36F3668DFB5EE3EA3FB32C9518333D00A6848B935D45D8898554332D78A5456C6BCA54EB867DBED2E994A737EB5143DDF2973ED46803930D9BE482FB3BBB63D9058E2D508FDF97B9154E6D722FB20A59EC2CDDAAE9867DA5B354BE6D9ED9D1A9050D353F0681469FA51210D8BF9FE1D8810451652799FAA8CA11330039B1E533A834CB6CD7EFA868F56DC40E7DA6AE1741FB632FFF0C024D3A14EC369CC05504651657F9AFC9F53300198F132C0B958C70ACCAB1C8627D892EFC486A74C19AAEA579DD973020EA11F42B4FC2ADF33983713908F10E41FB4EF8E47F287209289DB85D7111C0F7B2B1C72CAA645A5DEE330294C052621D973E90D703E3B1D5C6D5925A34B807A344F6E712489CE57B70F8228700B0B993AE80AD0A932A887F1CCAFED65907A421F4309D4C3708FA9832FF8D98894E26B8034ABD6EF51DDAD5A5D9780ABB0D0295047DD2DF189B6E99B389D2AFB2675D47D13EF5A9BBE3B39A2ECBAABA2EEB9F3D2D4754C0E41426FE43BD44577EED3421EADDF02F0B10882DF971A2C33626411D718290017585BA607DED9ED04D0DD6708302E31034BECB7206C5226EB80D89D75BD30671BA11FA614EA89399DE9FAEA1468A193EE33041D97180C61D0EF45FC87221079526A8039ABE6894360CBC1B1D055F43D0E6F6BC5CEC622A89FBED45C9468C4884E84188A0FF25A1210207D092842BA42E32D4E26A5D862C5666728B3F85B6A5835D0ECE06C1DC34E6523A40BA59D4946C7A697573F189A5195E1CD1D7C5AC45ED7C0FA42D33750266889CAD7441424A094BF106407EE40947ED7D613057A3DB375A230DA894014521A9C284422E86902BC0FD93A4968692750A42BF42688F8B2012086E6F9C35608C11D04DAF6C3B760C3661DF715435778F83FE1E1B3B632C5F015463557B4A176CC8E41372605A1074EEC3CFA81430621C82444610EA2BCD3418F4714F978656EB3A05F3085F2F871674CCDFBB102A354BABA32A8D227A516CFEE836270C221686846BE7A0F9076D5040627F5E4F41C187B226DDBF49F82B1660F50CE98B0579F0B9A13B224E068074D9CC61D8F1D26AFC977231DBFEE6CB03AE73148CBB2F13783CCA8F4EEDA7D510C1F3E098D8DA5440C490CF99663E527158C27A6278A56E9D0FAF184E10471DFA53F871DAC6ED6B7A9664C3758F68A4D3656B99B84F7BC0257846D4BE6BBF76059033F304E850780C40780C675F8A818287D3FD9B6EA3EF80F0D345B4343D4DBB7831C0CC0EBCC6EC46C89F7D0017B2C306E9DD55661B7050EF2D0217E92EB803E18CF606E1CCA0E17DD951CF9D0FCAC8BB2A1F07991A0AC6ABF1E2E3E6C721C0EB7FBF516E180DD0388C331D4F208B4AF8343D8F756560EA3BE0A179FE81CD55173EC8E8ECB3ABD8BE2BA29C679E6DB6BD83664DBD1FC74758B92657EB1A9D79BBA19325ADD664CD26B6CAD55F57FB810703EBC58B7022AC4101A34537C737091BFD9A4F82440F07E0704499280C0666012340FCF658D83E7DD3F0E90DE17B9212042BEC17A3D18A62EF2AB0827D2B1C7AD61BF33741FC58F974316701910FD44B0643F7C9B46F765B4AA088CB17DF3B3E1E164F5ED4FFF0F07B5CC530D8E0100 , N'6.1.3-40302')
 
*/