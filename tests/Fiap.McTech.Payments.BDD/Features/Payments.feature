Feature: Gerar QR-Code para pagamento

	Scenario: Gerar_QRCode_Pagamento
		Given que o valor de OrderId é um UUID e que os dados do objeto da requisição sejam ficticios
		When eu solicitar para o método GenerateQRCode e passar os parâmetros
		Then o status de resposta deve ser 200 OK