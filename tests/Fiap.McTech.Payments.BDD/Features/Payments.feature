Feature: Ações de gerar e atualizar um pagamento

	Scenario: Gerar_QRCode_Pagamento
		Given que o valor de OrderId é um UUID e que os dados do objeto da requisição sejam ficticios
		When eu solicitar para o método GenerateQRCode e passar os parâmetros
		Then o status de resposta deve ser 200 OK

	Scenario: Atualizar_Pagamento
		Given que o status informado para o pagamento é inválido
		When eu solicitar para atualizar o pagamento
		Then o sistema irá retornar erro