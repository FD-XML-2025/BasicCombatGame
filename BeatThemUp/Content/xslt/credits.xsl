<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0"
                xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
                xmlns:yr="http://www.yakuzasrevenge.fr/credits">
    <xsl:output method="html" encoding="UTF-8"/>
    <xsl:template match="/">
        <html>
            <head>
                <title>Document</title>
            </head>
            <body>
                <h1 style="text-align:center;">Credits</h1>
                <div style="text-align: center;">
                    <ul style="display: inline-block; text-align: left;">
                        <xsl:apply-templates select="yr:credits/yr:credit"/>
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
            <br/>
            <a href="{yr:github}" target="_blank">
                <xsl:value-of select="yr:github"/>
            </a>
            <br/>
            <a href="{yr:linkedin}" target="_blank">
                <xsl:value-of select="yr:linkedin"/>
            </a>
        </li>
    </xsl:template>
</xsl:stylesheet>