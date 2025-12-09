<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0"
                xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
                xmlns:yr="http://www.yakuzasrevenge.fr/credits">

    <xsl:output method="html" encoding="UTF-8" indent="yes"/>

    <xsl:template match="/">
        <html>
            <head>
                <title>Credits</title>
                <link rel="stylesheet" type="text/css" href="../css/credits.css"/>
            </head>
            <body>
                <div class="container">
                    <h1>Credits</h1>
                    <ul>
                        <xsl:apply-templates select="//yr:credit"/>
                    </ul>
                </div>
            </body>
        </html>
    </xsl:template>

    <xsl:template match="yr:credit">
        <li>
            <strong>
                <xsl:value-of select="yr:name"/>
            </strong>

            <xsl:if test="yr:github != ''">
                <a href="{yr:github}" target="_blank">Github</a>
            </xsl:if>

            <xsl:if test="yr:linkedin != ''">
                <a href="{yr:linkedin}" target="_blank">LinkedIn</a>
            </xsl:if>
        </li>
    </xsl:template>

</xsl:stylesheet>